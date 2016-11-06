using System;
using System.Configuration;
using System.Text;

namespace SlackBotV3
{
	public interface IBotTokenProvider
	{
		string GetBotToken();
	}

	public class BotTokenProvider : IBotTokenProvider
	{
		public string GetBotToken()
		{
			var botTokenKey = "botTokenBase64String";
			var botTokenBase64String = ConfigurationManager.AppSettings.Get(botTokenKey);
			var botTokenBytes = Convert.FromBase64String(botTokenBase64String);

			return Encoding.ASCII.GetString(botTokenBytes);
		}
	}
}
