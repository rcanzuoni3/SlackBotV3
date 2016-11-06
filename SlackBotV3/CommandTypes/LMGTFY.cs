using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	public class LMGTFYType : ICommandType
	{
		private ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "lmgtfy" }; }
		public string Help(string commandName) { return "Let me google that for you"; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public CommandScope GetCommandScope() { return CommandScope.Global; }
		public Type GetCommandHandlerType() { return typeof(LMGTFY); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }

		public LMGTFYType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}

	public class LMGTFY : ICommandHandler
	{
		private SlackBotV3 slackBot;

		public LMGTFY(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		public bool Execute(SlackBotCommand command)
		{
			slackBot.Reply(command, "http://lmgtfy.com/?q=" + System.Uri.EscapeDataString(command.Text));
			return false;
		}
	}
}
