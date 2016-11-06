using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	class RestartType : ICommandType
	{
		private ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "reboot" }; }
		public string Help(string commandName) { return "Kills Slackbot ಥ_ಥ , then does a pull and an update. Then restarts Slackbot :D"; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Super; }
		public CommandScope GetCommandScope() { return CommandScope.Global; }
		public Type GetCommandHandlerType() { return typeof(Restart); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }

		public RestartType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}

	public class Restart : ICommandHandler
	{
		private SlackBotV3 slackBot;

		public Restart(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		public bool Execute(SlackBotCommand command)
		{
			try
			{
				slackBot.Reply(command, "This feature is temporarily out of order.");
				//System.Diagnostics.Process.Start(@"C:\Users\plafata\Documents\SlackbotProd\reboot.bat");
			}
			catch (Exception)
			{
				slackBot.Reply(command, "Something went wrong restarting the bot");
			}
			return false;
		}
	}
}
