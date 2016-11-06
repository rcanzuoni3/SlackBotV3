using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlackBotV3.CommandTypes
{
	public class TicTacToeType : ICommandType
	{
		private ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "ttt" }; }
		public string Help(string commandName) { return "Tic-Tac-Toe! A Game of X's and O's!"; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public CommandScope GetCommandScope() { return CommandScope.Channel; }
		public Type GetCommandHandlerType() { return typeof(TicTacToe); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }

		public TicTacToeType() : this(new CommandHandlerProvider()) { }

		public TicTacToeType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}

	public class TicTacToe : ICommandHandler
	{
		private static List<Game> _games = new List<Game>();
		private const string NewGameUsage = "You aren't in a game right now, {0}. To make a new one, use the following command (Players can only be in one game at a time because that is what slackBot commands.)\n"
			+ "@ttt opponent [your symbol] [opponent's symbol]";
		private const string MoveUsage = "You are currently in a game, {0}. Here's the command to make a move:\n@ttt (tl|t|tr|ml|m|mr|bl|b|br|quit)";

		private SlackBotV3 slackBot;

		public TicTacToe(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		public bool Execute(SlackBotCommand command)
		{
			string[] splitCommand = command.Text.Split(' ');
			if (splitCommand[0] == "")
			{
				slackBot.Reply(command, "Try \"@ttt help\" to get started.");
				return false;
			}

			bool printHelp = splitCommand.Length == 1 && splitCommand[0] == "help";
			lock (_games)
			{
				int gameIndex = PlayerInGame(command.User.name);
				if (gameIndex < 0)
				{
					if (printHelp)
					{
						slackBot.Reply(command, String.Format(NewGameUsage, command.User.name));
						return true;
					}

					try
					{
						string opponentId;
						if (splitCommand[0].StartsWith("<@") && splitCommand[0].EndsWith(">"))
						{
							opponentId = splitCommand[0].Substring(2, splitCommand[0].Length - 3);
							if (Array.Find(command.Channel.members, userId => opponentId == userId) == null)
							{
								slackBot.Reply(command, String.Format("Couldn't find user: {0}.", splitCommand[0]));
								return false;
							}
						}
						else
						{
							opponentId = Array.Find(command.Channel.members,
								userId => slackBot.GetUser(userId) != null && slackBot.GetUser(userId).name == splitCommand[0]);
							if (opponentId == null)
							{
								slackBot.Reply(command, String.Format("Couldn't find user: {0}.", splitCommand[0]));
								return false;
							}
						}
						opponentId = slackBot.GetUser(opponentId).name;

						Game newGame = null;
						if (splitCommand.Length == 1 && PlayerInGame(opponentId) < 0)
							newGame = new Game(command.User.name, opponentId);
						else if (splitCommand.Length == 2 && PlayerInGame(opponentId) < 0)
							newGame = new Game(command.User.name, opponentId, splitCommand[2]);
						else if (splitCommand.Length == 3 && PlayerInGame(opponentId) < 0)
							newGame = new Game(command.User.name, opponentId, splitCommand[1], splitCommand[2]);
						else
							slackBot.Reply(command,
								command.User.name +
								", you aren't in a game right now and you're not good at making a new one either.\nUse \"@ttt help\" for help.");

						if (newGame != null)
						{
							_games.Add(newGame);
							slackBot.Reply(command, newGame.ToString());
						}
					}
					catch (Exception e)
					{
						slackBot.Reply(command, e.Message);
						return false;
					}

				}
				else
				{
					if (printHelp)
					{
						slackBot.Reply(command, String.Format(MoveUsage, command.User.name));
						return true;
					}

					if (splitCommand.Length != 1)
						slackBot.Reply(command, ":doge: WOW! Such wrong. :doge: Refer to \"@ttt help\"");
					else
					{
						if (splitCommand[0] == "quit")
						{
							slackBot.Reply(command,
								String.Format("{0} has quit the game. Probably because they were about to lose.", command.User.name));
							_games.RemoveAt(gameIndex);
						}
						else if (_games[gameIndex].ProcessMove(command.User.name, splitCommand[0]))
						{
							slackBot.Reply(command, _games[gameIndex].ToString());
							string winner = _games[gameIndex].EvaluateWinner();
							if (winner != null)
							{
								slackBot.Reply(command, String.Format("Game Over! The winner is: {0}!", winner));
								_games.RemoveAt(gameIndex);
							}
						}
						else
							slackBot.Reply(command, "Invalid Move, " + command.User.name);
					}
				}
				return true;
			}
		}

		private static int PlayerInGame(string playerName)
		{
			for (int i = 0; i < _games.Count; i++)
			{
				if (_games[i].IsPlayerInGame(playerName))
					return i;
			}
			return -1;
		}

		private class Game
		{
			private Dictionary<string, string> players;
			private int currPlayer;
			private string[,] grid = new string[3, 3] { { "", "", "" }, { "", "", "" }, { "", "", "" } };
			private int movesRemaining = 9;
			private List<Tuple<Tuple<int, int>, Tuple<int, int>, Tuple<int, int>>> winningPositions = new List<Tuple<Tuple<int, int>, Tuple<int, int>, Tuple<int, int>>>()
			{
				new Tuple<Tuple<int,int>, Tuple<int,int>, Tuple<int,int>>(new Tuple<int, int>(0,0), new Tuple<int, int>(0,1), new Tuple<int, int>(0,2)),
				new Tuple<Tuple<int,int>, Tuple<int,int>, Tuple<int,int>>(new Tuple<int, int>(1,0), new Tuple<int, int>(1,1), new Tuple<int, int>(1,2)),
				new Tuple<Tuple<int,int>, Tuple<int,int>, Tuple<int,int>>(new Tuple<int, int>(2,0), new Tuple<int, int>(2,1), new Tuple<int, int>(2,2)),
				new Tuple<Tuple<int,int>, Tuple<int,int>, Tuple<int,int>>(new Tuple<int, int>(0,0), new Tuple<int, int>(1,0), new Tuple<int, int>(2,0)),
				new Tuple<Tuple<int,int>, Tuple<int,int>, Tuple<int,int>>(new Tuple<int, int>(0,1), new Tuple<int, int>(1,1), new Tuple<int, int>(2,1)),
				new Tuple<Tuple<int,int>, Tuple<int,int>, Tuple<int,int>>(new Tuple<int, int>(0,2), new Tuple<int, int>(1,2), new Tuple<int, int>(2,2)),
				new Tuple<Tuple<int,int>, Tuple<int,int>, Tuple<int,int>>(new Tuple<int, int>(0,0), new Tuple<int, int>(1,1), new Tuple<int, int>(2,2)),
				new Tuple<Tuple<int,int>, Tuple<int,int>, Tuple<int,int>>(new Tuple<int, int>(2,0), new Tuple<int, int>(1,1), new Tuple<int, int>(0,2))
			};

			public Game(string player1, string player2) : this(player1, player2, ":heavy_multiplication_x:", ":white_circle:") { }

			public Game(string player1, string player2, string player1Mark) : this(player1, player2, player1Mark, ":white_circle:") { }

			public Game(string player1, string player2, string player1Mark, string player2Mark)
			{
				if (player1 == player2)
					throw new Exception("You can't play with yourself at work!");

				players = new Dictionary<string, string>();
				if (player1Mark == player2Mark)
				{
					players.Add(player1, ":heavy_multiplication_x:");
					players.Add(player2, ":white_circle:");
				}
				else
				{
					players.Add(player1, player1Mark);
					players.Add(player2, player2Mark);
				}

				currPlayer = new Random().Next(2);
			}

			public bool IsPlayerInGame(string playerName)
			{
				return players.ContainsKey(playerName);
			}

			public string EvaluateWinner()
			{
				string winner = null;
				foreach (var winSet in winningPositions)
				{
					string temp = grid[winSet.Item1.Item1, winSet.Item1.Item2];
					if (!String.IsNullOrEmpty(temp) && temp == grid[winSet.Item2.Item1, winSet.Item2.Item2] && temp == grid[winSet.Item3.Item1, winSet.Item3.Item2])
						winner = temp;
				}

				if (winner == null && movesRemaining == 0)
					return "nobody";

				return winner;
			}

			public bool ProcessMove(string playerName, string move)
			{
				if (playerName != players.Keys.ToList<string>()[currPlayer])
					return false;

				bool validMove = false;
				switch (move)
				{
					case "tl":
						if (String.IsNullOrEmpty(grid[0, 0]))
						{
							grid[0, 0] = players[playerName];
							validMove = true;
						}
						break;
					case "t":
						if (String.IsNullOrEmpty(grid[0, 1]))
						{
							grid[0, 1] = players[playerName];
							validMove = true;
						}
						break;
					case "tr":
						if (String.IsNullOrEmpty(grid[0, 2]))
						{
							grid[0, 2] = players[playerName];
							validMove = true;
						}
						break;
					case "ml":
						if (String.IsNullOrEmpty(grid[1, 0]))
						{
							grid[1, 0] = players[playerName];
							validMove = true;
						}
						break;
					case "m":
						if (String.IsNullOrEmpty(grid[1, 1]))
						{
							grid[1, 1] = players[playerName];
							validMove = true;
						}
						break;
					case "mr":
						if (String.IsNullOrEmpty(grid[1, 2]))
						{
							grid[1, 2] = players[playerName];
							validMove = true;
						}
						break;
					case "bl":
						if (String.IsNullOrEmpty(grid[2, 0]))
						{
							grid[2, 0] = players[playerName];
							validMove = true;
						}
						break;
					case "b":
						if (String.IsNullOrEmpty(grid[2, 1]))
						{
							grid[2, 1] = players[playerName];
							validMove = true;
						}
						break;
					case "br":
						if (String.IsNullOrEmpty(grid[2, 2]))
						{
							grid[2, 2] = players[playerName];
							validMove = true;
						}
						break;
					default:
						return false;
				}

				if (validMove)
				{
					currPlayer = currPlayer ^ 1;
					movesRemaining--;
				}

				return validMove;
			}

			public override string ToString()
			{
				StringBuilder output = new StringBuilder();

				string[] playerNames = players.Keys.ToArray<string>();
				output.AppendLine(String.Format("{0} ({1}) vs. {2} ({3})", playerNames[0], players[playerNames[0]], playerNames[1], players[playerNames[1]]));
				output.AppendLine(String.Format("Your move, {0}!", playerNames[currPlayer]));

				output.AppendLine(String.Format("{0}:filled:{1}:filled:{2}",
					String.IsNullOrEmpty(grid[0, 0]) ? ":blank:" : grid[0, 0],
					String.IsNullOrEmpty(grid[0, 1]) ? ":blank:" : grid[0, 1],
					String.IsNullOrEmpty(grid[0, 2]) ? ":blank:" : grid[0, 2]));

				output.AppendLine(":filled::filled::filled::filled::filled:");

				output.AppendLine(String.Format("{0}:filled:{1}:filled:{2}",
					String.IsNullOrEmpty(grid[1, 0]) ? ":blank:" : grid[1, 0],
					String.IsNullOrEmpty(grid[1, 1]) ? ":blank:" : grid[1, 1],
					String.IsNullOrEmpty(grid[1, 2]) ? ":blank:" : grid[1, 2]));

				output.AppendLine(":filled::filled::filled::filled::filled:");

				output.AppendLine(String.Format("{0}:filled:{1}:filled:{2}",
					String.IsNullOrEmpty(grid[2, 0]) ? ":blank:" : grid[2, 0],
					String.IsNullOrEmpty(grid[2, 1]) ? ":blank:" : grid[2, 1],
					String.IsNullOrEmpty(grid[2, 2]) ? ":blank:" : grid[2, 2]));

				return output.ToString();
			}
		}
	}
}