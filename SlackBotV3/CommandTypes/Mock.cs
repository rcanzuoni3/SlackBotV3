using System;
using System.Collections.Generic;
using SlackAPI;

namespace SlackBotV3.CommandTypes
{
	class MockType : ICommandType
	{
		private ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "mock" }; }
		public string Help(string commandName) { return "Type mock followed by the username or real name of the user you want SlackBot to mock."; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public CommandScope GetCommandScope() { return CommandScope.Global; }
		public Type GetCommandHandlerType() { return typeof(Mock); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }
		public MockType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}

	public class Mock : ICommandHandler
	{
		private SlackBotV3 slackBot;
		public Mock(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		public bool Execute(SlackBotCommand command)
		{
			User userToMock;

			if (string.IsNullOrWhiteSpace(command.Text))
				userToMock = command.User;
			else
			{
				string userId = command.Text;
				userToMock = slackBot.GetUsers().Find(user => user.name == userId);
				if (userToMock == null)
					userToMock = slackBot.GetUsers().Find(user => user.profile.real_name == userId);

				if (userToMock == null)
				{
					userId = userId.Replace("\"", "");
					userId = "\"" + userId + "\"";
					slackBot.Reply(command, string.Format("Sorry {0}, I cannot find user {1}", command.User.name, userId));
					return false; ;
				}
			}

			string userName;
			if (!string.IsNullOrWhiteSpace(userToMock.profile.real_name))
				userName = userToMock.profile.real_name;
			else
				userName = userToMock.name;

			slackBot.Reply(command, string.Format("I'm {0} and I'm dumb", userName), userName, userToMock.profile.image_32);
			return false;
		}
	}
}
