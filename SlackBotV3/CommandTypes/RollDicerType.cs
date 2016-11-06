using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	class RollDicerType : ICommandType
	{
		private ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "dice" }; }
		public string Help(string commandName) { return "Type dice to dice the rolls"; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public CommandScope GetCommandScope() { return CommandScope.Global; }
		public Type GetCommandHandlerType() { return typeof(RollDicer); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }

		public RollDicerType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}
	public class RollDicer : ICommandHandler
	{
		private SlackBotV3 slackBot;

		public RollDicer(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		public bool Execute(SlackBotCommand command)
		{
			Random random = new Random();
			int slicesX = random.Next(5) + 1;
			int slicesY = random.Next(5) + 1;
			int slicesZ = random.Next(5) + 1;

			int pieces = slicesX * slicesY * slicesZ;

			slackBot.Reply(command, string.Format("{0} diced the rolls into {1} pieces", command.User.name, pieces), iconUrl: "https://upload.wikimedia.org/wikipedia/commons/2/28/13-08-31-wien-redaktionstreffen-EuT-by-Bi-frie-134.jpg");

			return false;
		}
	}
}