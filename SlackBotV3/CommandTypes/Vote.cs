using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	public class VoteType : ICommandType
	{
		private ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "makevote", "vote" }; }
		public string Help(string commandName)
		{
			if (commandName == "makevote")
				return "Type makevote followed by a topic to vote on.  Only one vote in a channel at a time." +
					"The vote ends when a majority of active non-bot users have voted.";
			else
				return "Type vote followed by yes or no.";
		}
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public CommandScope GetCommandScope() { return CommandScope.Channel; }
		public Type GetCommandHandlerType() { return typeof(Vote); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }

		public VoteType() : this(new CommandHandlerProvider()) { }

		public VoteType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}

	public class Vote : ICommandHandler
	{
		private HashSet<string> Voted = new HashSet<string>();
		private int YesVotes = 0;
		private int NoVotes = 0;
		private string VotingTopic = null;
		private SlackBotV3 slackBot;

		public Vote(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		public bool Execute(SlackBotCommand command)
		{
			if (command.Name == "makevote")
			{
				if (string.IsNullOrWhiteSpace(command.Text))
				{
					slackBot.Reply(command, "What do you want to vote on?");
					return false;
				}
				else if (VotingTopic != null)
				{
					slackBot.Reply(command, "There is already an ongoing vote.");
					return true;
				}

				VotingTopic = command.Text;
				return true;
			}
			else
			{
				if (VotingTopic == null)
				{
					slackBot.Reply(command, "There is no vote going on currently");
					return false;
				}
				if (!(command.Text == "yes" || command.Text == "no"))
				{
					slackBot.Reply(command, "You must vote 'yes' or 'no'");
					return true;
				}
				else if (Voted.Contains(command.User.id))
				{
					slackBot.Reply(command, "You already voted");
					return true;
				}

				if (command.Text == "yes")
					YesVotes++;
				else
					NoVotes++;
				Voted.Add(command.User.id);

				int difference = Math.Abs(YesVotes - NoVotes);
				int votes = YesVotes + NoVotes;
				int votesLeft = Array.FindAll(command.Channel.members, userId => slackBot.GetUser(userId).presence == "active" && !slackBot.GetUser(userId).is_bot).Length - votes;

				if (difference > votesLeft)
				{
					string result = (YesVotes > NoVotes) ? "yes" : "no";
					slackBot.Reply(command, "The people have spoken, the vote is " + result + " to " + VotingTopic);
					return false;
				}
				else if (votesLeft == 0)
				{
					slackBot.Reply(command, "The vote was inconclusive.");
					return false;
				}

				return true;
			}
		}
	}
}