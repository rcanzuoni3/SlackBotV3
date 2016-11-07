using System;
using System.Text;

namespace SlackBotV3
{
	public interface IBotTokenProvider
	{
		string GetBotToken(string botTokenKey);
	}

	public class BotTokenProvider : IBotTokenProvider
	{
		IAppConfigValueRetriever appConfigValueRetriever;

		public BotTokenProvider(IAppConfigValueRetriever appConfigValueRetriever)
		{
			this.appConfigValueRetriever = appConfigValueRetriever;
		}

		public string GetBotToken(string botTokenKey)
		{
			var botTokenBase64String = appConfigValueRetriever.GetValue(botTokenKey);
			var botTokenBytes = Convert.FromBase64String(botTokenBase64String);

			return Encoding.ASCII.GetString(botTokenBytes);
		}
	}
}
