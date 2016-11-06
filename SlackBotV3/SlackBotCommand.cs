using System;
using SlackAPI;
using SlackAPI.WebSocketMessages;

namespace SlackBotV3
{
	public class SlackBotCommand
	{
		private const string CommandPrefix = "@";

		public static bool GetCommand(NewMessage newMessage, SlackSocketClient slackBot, out SlackBotCommand command)
		{
			command = new SlackBotCommand(newMessage, slackBot);
			return newMessage.text.StartsWith(CommandPrefix);
		}

		private SlackBotCommand(NewMessage message, SlackSocketClient slackBot)
		{
			user = slackBot.UserLookup.ContainsKey(message.user) ? slackBot.UserLookup[message.user] : null;

			channel = null;
			if (slackBot.ChannelLookup.ContainsKey(message.channel))
				channel = slackBot.ChannelLookup[message.channel];
			else if (slackBot.GroupLookup.ContainsKey(message.channel))
				channel = slackBot.GroupLookup[message.channel];

			string text = message.text.Substring(message.text.IndexOf(CommandPrefix) + CommandPrefix.Length).NormalizeSpace();
			int firstSpace = text.IndexOf(' ');
			if (firstSpace != -1)
			{
				command = text.Substring(0, firstSpace);
				commandText = text.Substring(firstSpace + 1);
			}
			else
			{
				command = text;
				commandText = string.Empty;
			}

			timestamp = message.ts;
		}

		private User user;
		public User User { get { return user; } }

		private Channel channel;
		public Channel Channel { get { return channel; } }

		private string commandText;
		public string Text { get { return commandText; } }

		private string command;
		public string Name { get { return command; } }

		private DateTime timestamp;
		public DateTime Timestamp { get { return timestamp; } }
	}
}
