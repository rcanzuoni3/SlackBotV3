using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	public class RenameCommandType : ICommandType
	{
		private ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "rename" }; }
		public string Help(string commandName) { return "Type rename followed by the new bot name"; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Admin; }
		public CommandScope GetCommandScope() { return CommandScope.Global; }
		public Type GetCommandHandlerType() { return typeof(Rename); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }

		public RenameCommandType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}

	public class Rename : ICommandHandler
	{
		private SlackBotV3 slackBot;

		public Rename(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		public bool Execute(SlackBotCommand command)
		{
			if (string.IsNullOrWhiteSpace(command.Text))
				slackBot.Reply(command, "You did not specify a new name");
			else
				slackBot.RenameBot(command.Text);

			return false;
		}
	}
}