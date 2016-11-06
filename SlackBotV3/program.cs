using System;

namespace SlackBotV3
{
	public class Program
	{
		private IBotTokenProvider botTokenProvider;
		private const string botTokenKey = "botTokenBase64String";

		public Program(IBotTokenProvider botTokenProvider)
		{
			this.botTokenProvider = botTokenProvider;
		}

		public void RunProgram()
		{
			var slackToken = botTokenProvider.GetBotToken(botTokenKey);
			SlackBotV3 slackBot = new SlackBotV3(slackToken);

			slackBot.Connect();

			Console.ReadLine();
		}
	}
}
