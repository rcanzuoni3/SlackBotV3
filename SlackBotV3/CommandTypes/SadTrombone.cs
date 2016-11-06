using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	class SadTromboneType : ICommandType
	{
		private ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "sadtrombone" }; }
		public string Help(string commandName) { return "Plays a sad trombone"; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public CommandScope GetCommandScope() { return CommandScope.Global; }
		public Type GetCommandHandlerType() { return typeof(SadTrombone); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }

		public SadTromboneType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}

	public class SadTrombone : ICommandHandler
	{
		private SlackBotV3 slackBot;

		public SadTrombone(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		public bool Execute(SlackBotCommand command)
		{
			slackBot.Reply(command, "http://www.sadtrombone.com/");
			return false;
		}
	}
}

