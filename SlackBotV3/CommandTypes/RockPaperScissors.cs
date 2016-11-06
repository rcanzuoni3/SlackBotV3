using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	public class RockPaperScissorsType : ICommandType
	{
		private ICommandHandlerProvider commandHandlerProvider;

		public  List<string> CommandNames() { return new List<string>() { "rps", "rock", "paper", "scissors" }; }
		public  string Help(string commandName) { return "Type command followed by username of opponent"; }
		public  PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public  CommandScope GetCommandScope() { return CommandScope.Channel; }
		public  Type GetCommandHandlerType() { return typeof(RockPaperScissors); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }

		public RockPaperScissorsType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}
	public class RockPaperScissors : ICommandHandler
	{
		private static Dictionary<string, string> m_ObjectBeats = new Dictionary<string, string>
			{
				{"rock", "scissors"},
				{"paper", "rock"},
				{"scissors", "paper"}
			};

		private SlackBotV3 slackBot;

		public RockPaperScissors(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		private string[] m_Players = null;//id of players
		private Dictionary<string, string> m_Moves = null;
		private Dictionary<string, int> m_FalseStartCount;
		private DateTime m_ShootTime = DateTime.MinValue;
		private bool Initialize(SlackBotCommand command)
		{
			m_Players = new string[2];
			m_Moves = new Dictionary<string, string>();
			m_Players[0] = command.User.id;

			string opponentId;
			if (command.Text.StartsWith("<@") && command.Text.EndsWith(">"))
			{
				opponentId = command.Text.Substring(2, command.Text.Length - 3);
				if (Array.Find(command.Channel.members, userId => opponentId == userId) == null)
					return false;
			}
			else
			{
				opponentId = Array.Find(command.Channel.members, userId => slackBot.GetUser(userId) != null && slackBot.GetUser(userId).name == command.Text);
				if (opponentId == null)
					return false;
			}

			m_Players[1] = opponentId;

			m_FalseStartCount = new Dictionary<string, int>()
				{
					{m_Players[0], 0},
					{m_Players[1], 0}
				};

			return true;
		}

		private void Run(SlackBotCommand command)
		{
			m_Moves.Clear();
			foreach (string shout in new string[] { "Rock!", "Paper!", "Scissors!", "Shoot!" })
			{
				System.Threading.Thread.Sleep(1000);
				slackBot.Reply(command, shout);
			}

			m_ShootTime = DateTime.Now;
		}

		public bool Execute(SlackBotCommand command)
		{
			if (command.Name == "rps")
			{
				if (m_Players != null)
					return true;

				if (!Initialize(command))
				{
					slackBot.Reply(command, "Sorry, could not find user " + command.Text);
					return false;
				}

				Run(command);
				return true;
			}
			else
			{
				if (m_Players == null)
				{
					slackBot.Reply(command, "There is no game going on currently.");
					return false;
				}

				if (!new HashSet<string> { "rock", "paper", "scissors" }.Contains(command.Name))
				{
					slackBot.Reply(command, command.Text + " is not a valid move");
					return true;
				}

				if (!new HashSet<string>(m_Players).Contains(command.User.id))
				{
					return true;
				}

				if (command.Timestamp < m_ShootTime.AddSeconds(-3)) //If the command was sent more than three seconds before the shoot time then
					return true; //it most likely was meant for the previous round.  Just ignore it

				bool earlyStart = command.Timestamp < m_ShootTime;
				bool lateStart = command.Timestamp > m_ShootTime.AddMilliseconds(800);
				bool falseStart = earlyStart || lateStart;

				if (falseStart)
				{
					m_FalseStartCount[command.User.id]++;
					System.Threading.Thread.Sleep(1000);

					if (m_FalseStartCount[command.User.id] >= 3)
					{
						if (earlyStart)
							slackBot.Reply(command, command.User.name + " went too early and is disqualified");
						else
							slackBot.Reply(command, command.User.name + " went too late and is disqualified");

						return false;
					}
					else
					{
						if (earlyStart)
							slackBot.Reply(command, command.User.name + " went too early. Trying again.");
						else
							slackBot.Reply(command, command.User.name + " went too late. Trying again.");

						Run(command);
						return true;
					}
				}

				m_Moves[command.User.id] = command.Name;

				if (m_Moves.Count == 2)
				{
					if (m_Moves[m_Players[0]] == m_Moves[m_Players[1]])
					{
						Run(command);
						return true;
					}

					int winner = 0;
					if (m_ObjectBeats[m_Moves[m_Players[0]]] == m_Moves[m_Players[1]])
						winner = 0;
					else
						winner = 1;

					slackBot.Reply(command, m_Moves[m_Players[winner]] + " beats " + m_Moves[m_Players[(winner + 1) % 2]] + ". " + slackBot.GetUser(m_Players[winner]).name + " is the winner.");

					return false;
				}

				return true;
			}

		}
	}
}