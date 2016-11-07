using System.Collections.Generic;
using System.Configuration;

namespace SlackBotV3
{
	public interface IAppConfigValueRetriever
	{
		string GetValue(string key);
	}

	public class AppConfigValueRetriever : IAppConfigValueRetriever
	{
		public string GetValue(string key)
		{
			var value = ConfigurationManager.AppSettings.Get(key);

			if (string.IsNullOrWhiteSpace(value))
				throw new KeyNotFoundException(string.Format("Could not fine key {0} in the app.config", key));

			return value;
		}
	}
}
