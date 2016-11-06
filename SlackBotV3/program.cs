using System;

namespace SlackBotV3
{
	public class Program
	{
		private SlackBotV3 slackBot;

		public Program(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		public void RunProgram()
		{
			slackBot.Connect();
			Console.ReadLine();
		}
	}
}
