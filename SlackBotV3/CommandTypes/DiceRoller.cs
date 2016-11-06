using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandHandlers
{
	public class DiceRollerType : ICommandType
	{
		ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "roll" }; }
		public string Help(string commandName) { return "Type roll to roll the dice"; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public CommandScope GetCommandScope() { return CommandScope.Global; }
		public Type GetCommandHandlerType() { return typeof(DiceRoller); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }

		public DiceRollerType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}

	public class DiceRoller : ICommandHandler
	{
		private SlackBotV3 slackBot;

		public DiceRoller(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		public bool Execute(SlackBotCommand command)
		{
			Random random = new Random();
			int die1 = random.Next(6) + 1;
			int die2 = random.Next(6) + 1;

			int roll = die1 + die2;

			slackBot.Reply(command, string.Format("{0} rolled a {1}", command.User.name, roll), iconUrl: "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c4/2-Dice-Icon.svg/2000px-2-Dice-Icon.svg.png");

			return false;
		}
	}
}
