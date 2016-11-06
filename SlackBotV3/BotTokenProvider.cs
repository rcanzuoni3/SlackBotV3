using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace SlackBotV3
{
	public interface IBotTokenProvider
	{
		string GetBotToken(string botTokenKey);
	}

	public class BotTokenProvider : IBotTokenProvider
	{
		public string GetBotToken(string botTokenKey)
		{
			var botTokenBase64String = ConfigurationManager.AppSettings.Get(botTokenKey);

			if (string.IsNullOrWhiteSpace(botTokenBase64String))
				throw new KeyNotFoundException("Could not fine key {0} in the app.config");

			var botTokenBytes = Convert.FromBase64String(botTokenBase64String);

			return Encoding.ASCII.GetString(botTokenBytes);
		}
	}
}
