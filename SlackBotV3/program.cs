using System;

namespace SlackBotV3
{
	public class Program
	{
		IBotTokenProvider botTokenProvider;

		public Program(IBotTokenProvider botTokenProvider)
		{
			this.botTokenProvider = botTokenProvider;
		}

		public void RunProgram()
		{
			var slackToken = botTokenProvider.GetBotToken();
			SlackBotV3 slackBot = new SlackBotV3(slackToken);

			slackBot.Connect();

			Console.ReadLine();
		}
	}
}
