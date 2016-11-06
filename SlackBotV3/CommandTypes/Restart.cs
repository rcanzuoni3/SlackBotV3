using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	class Restart : CommandType
	{
		public override List<string> CommandNames() { return new List<string>() { "reboot" }; }
		public override string Help(string commandName) { return "Kills Slackbot ಥ_ಥ , then does a pull and an update. Then restarts Slackbot :D"; }
		public override PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Super; }
		public override CommandScope GetCommandScope() { return CommandScope.Global; }

		public override Type GetCommandHandlerType()
		{
			return typeof(Advice);
		}

		class Advice : CommandHandler
		{
			public Advice(SlackBotV3 bot) : base(bot) { }

			public override bool Execute(SlackBotCommand command)	
			{
				try
				{					
					System.Diagnostics.Process.Start(@"C:\Users\plafata\Documents\SlackbotProd\reboot.bat");
				}
				catch (Exception)
				{
					SlackBot.Reply(command, "Something went wrong restarting the bot");
				}
				return false;
			}
		}
	}
}
