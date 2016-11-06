using System;
using System.Collections.Generic;
using System.Linq;

namespace SlackBotV3.CommandHandlers
{
	class HelpType : CommandType
	{
		public override List<string> CommandNames() { return new List<string>() { "help" }; }
		public override string Help(string commandName) { return "Get the help for a command: @help [command name]"; }
		public override PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public override CommandScope GetCommandScope() { return CommandScope.Global; }

		public override Type GetCommandHandlerType()
		{
			return typeof(HelpCommand);
		}

		class HelpCommand : CommandHandler
		{
			public HelpCommand(SlackBotV3 bot) : base(bot) { }

			public override bool Execute(SlackBotCommand command)
			{
				if (string.IsNullOrWhiteSpace(command.Text))
				{
					string response = "";
					foreach (string commandName in SlackBot.GetCommandNames().OrderBy(x => x))
					{
						response += commandName + ": ";
						response += SlackBot.GetHelp(commandName) + "\n";
					}

					SlackBot.Reply(command, response);
				}
				else
					SlackBot.Reply(command, SlackBot.GetHelp(command.Text));

				return true;
			}
		}
	}
}
