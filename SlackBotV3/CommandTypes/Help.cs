using System;
using System.Collections.Generic;
using System.Linq;

namespace SlackBotV3.CommandHandlers
{
	public class HelpType : ICommandType
	{
		private ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "help" }; }
		public string Help(string commandName) { return "Get the help for a command: @help [command name]"; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public CommandScope GetCommandScope() { return CommandScope.Global; }
		public Type GetCommandHandlerType() { return typeof(HelpCommand); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }

		public HelpType() : this(new CommandHandlerProvider()) { }

		public HelpType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}

	public class HelpCommand : ICommandHandler
	{
		private SlackBotV3 slackBot;

		public HelpCommand(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		public bool Execute(SlackBotCommand command)
		{
			if (string.IsNullOrWhiteSpace(command.Text))
			{
				string response = "";
				foreach (string commandName in slackBot.GetCommandNames().OrderBy(x => x))
				{
					response += commandName + ": ";
					response += slackBot.GetHelp(commandName) + "\n";
				}

				slackBot.Reply(command, response);
			}
			else
				slackBot.Reply(command, slackBot.GetHelp(command.Text));

			return true;
		}
	}
}
