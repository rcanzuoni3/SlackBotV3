using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandHandlers
{
	public class CheckPrivelegeType : ICommandType
	{
		ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "checkprivilege" }; }
		public string Help(string commandName) { return "checkprivilege [username] to check your privilege"; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public CommandScope GetCommandScope() { return CommandScope.Global; }
		public Type GetCommandHandlerType() { return typeof(CheckPrivilege); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }
		public CheckPrivelegeType() : this(new CommandHandlerProvider()) { }

		public CheckPrivelegeType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}

	public class CheckPrivilege : ICommandHandler
	{
		private SlackBotV3 slackBot;

		public CheckPrivilege(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		public bool Execute(SlackBotCommand command)
		{
			string userToCheck;
			if (string.IsNullOrWhiteSpace(command.Text))
				userToCheck = command.User.name;
			else
				userToCheck = command.Text;

			PrivilegeLevel privilege = slackBot.GetUserPrivilege(userToCheck);

			string response;
			switch (privilege)
			{
				case PrivilegeLevel.Super:
					response = "Your privelege level is Super, that makes you in the 1% of privilege levels"; break;
				case PrivilegeLevel.Admin:
					response = "You got some privilege but not a lot.  You're an admin which is kind of like the guy in the factory that watches the other guys work." +
					"On the one hand you're not the lowest level, but you're still in the factory son."; break;
				case PrivilegeLevel.Normal:
				default:
					response = "You're just a regular joe, p'ting.  But not like our Joe because he's actually kind of a big deal."; break;
			}

			slackBot.Reply(command, response);
			return false;
		}
	}
}

