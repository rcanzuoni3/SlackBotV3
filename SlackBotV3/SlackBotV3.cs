using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SlackAPI;
using SlackAPI.WebSocketMessages;

namespace SlackBotV3
{
	public class SlackBotV3
	{
		private const string IconUrl = "https://s3-us-west-2.amazonaws.com/slack-files2/avatars/2015-07-06/7287183376_98f07e9f81fa3dea7be9_72.jpg";
		private string BotName = "SlackBotV3";

		private HashSet<String> Admins = new HashSet<string>() { "ranzuoni", "lbrooks", "ahachten", "plafata", "nsmith" };
		private HashSet<string> Supers = new HashSet<string>() { "plafata", "lbrooks", "ranzuoni", "nsmith" };

		private CommandTypeRegistry CommandTypeRegistry = new CommandTypeRegistry();
		private CommandHandlerRegistry CommandHandlerRegistry;

		private SlackSocketClient SlackBot;

		public SlackBotV3(string token)
		{
			SlackBot = new SlackSocketClient(token);

			foreach (Type t in this.GetType().Assembly.GetTypes())
			{
				if (!t.IsAbstract && new HashSet<Type>(t.GetInterfaces()).Contains(typeof(ICommandType)))
				{
					ConstructorInfo constructorInfo = t.GetConstructor(new Type[] { });
					ICommandType commandType = (ICommandType)constructorInfo.Invoke(new object[] { });
					CommandTypeRegistry.RegisterCommandType(commandType);
				}
			}

			CommandHandlerRegistry = new CommandHandlerRegistry(CommandTypeRegistry);
		}

		public void Connect()
		{
			SlackBot.OnMessageReceived += HandleMessage;
			SlackBot.Connect((connected) => { }, () => { });
		}

		private void HandleMessage(NewMessage newMessage)
		{
			if (newMessage.user == null) //The message is from us, ignore
				newMessage.user = SlackBot.MySelf.id;
			//return;

			SlackBotCommand command;

			if (!SlackBotCommand.GetCommand(newMessage, SlackBot, out command))
				return;

			// DM to slackbot doesn't have a channel.
			if (command.Channel == null)
				return;

			if (CommandTypeRegistry.HasHandler(command.Name))
			{
				PrivilegeLevel privilegeLevel = CommandTypeRegistry.GetCommandType(command.Name).GetPrivilegeLevel();
				if ((int)GetUserPrivilege(command.User.name) < (int)privilegeLevel)
				{
					Reply(command, "http://i.imgur.com/egUwx5Q.gif");
					Reply(command, "You better check your privilege!");
					return;
				}

				if (!CommandHandlerRegistry.HasCommand(command))
					CommandHandlerRegistry.RegisterCommand(command, this);

				ICommandHandler handler = CommandHandlerRegistry.GetCommand(command);
				if (!handler.Execute(command))
					CommandHandlerRegistry.DeRegisterCommand(command);
			}
			else
			{
				Reply(command, ":clippy: \"Were you trying to run this command?\"");
				Reply(command, string.Format("@badjob {0} not knowing the commands.", command.User.name));
			}
		}

		public void RenameBot(string newName)
		{
			BotName = newName;
		}

		public User GetUser(string userId)
		{
			if (SlackBot.UserLookup.ContainsKey(userId))
				return SlackBot.UserLookup[userId];
			else
				return null;
		}

		public PrivilegeLevel GetUserPrivilege(string userId)
		{
			if (Supers.Contains(userId))
				return PrivilegeLevel.Super;
			if (Admins.Contains(userId))
				return PrivilegeLevel.Admin;

			return PrivilegeLevel.Normal;
		}

		public List<User> GetUsers()
		{
			return SlackBot.Users;
		}

		public string GetHelp(string commandName)
		{
			if (CommandTypeRegistry.HasHandler(commandName))
				return CommandTypeRegistry.GetCommandType(commandName).Help(commandName);
			return "Command not found!";
		}

		public IEnumerable<string> GetCommandNames()
		{
			return CommandTypeRegistry.GetCommandNames();
		}

		public void Reply(SlackBotCommand command, string response, string botName = null, string iconUrl = IconUrl, string channelID = null)
		{
			if (botName == null)
				botName = BotName;

			if (channelID == null)
				channelID = command.Channel.id;

			if (command.Channel.name == "the-the-the-the-the-")
			{
				int numThes = response.NormalizeSpace().Split(' ').Count();
				StringBuilder responseBuilder = new StringBuilder();
				for (int i = 0; i < numThes; ++i)
					responseBuilder.Append("the ");

				response = responseBuilder.ToString();
				botName = "TheTheTheTheThe";
			}

			SlackBot.PostMessage((messageReceived) => { }, channelID, response, botName, icon_url: iconUrl);
		}
	}
}
