using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	public class DefineType : ICommandType
	{
		ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "define" }; }
		public string Help(string commandName) { return "Type define followed by phrase to lookup defintion"; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public CommandScope GetCommandScope() { return CommandScope.Global; }
		public Type GetCommandHandlerType() { return typeof(Define); }

		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot)
		{
			throw new NotImplementedException();
		}

		public DefineType() : this(new CommandHandlerProvider()) { }

		public DefineType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}

	public class Define : ICommandHandler
	{
		private SlackBotV3 slackBot;

		public Define(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		public bool Execute(SlackBotCommand command)
		{
			slackBot.Reply(command, "http://www.urbandictionary.com/define.php?term=" + System.Uri.EscapeDataString(command.Text));
			return false;
		}
	}
}