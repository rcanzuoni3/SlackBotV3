using System;
using System.Text.RegularExpressions;
using System.Configuration;

using SlackAPI;
namespace SlackBotV3
{
	class Run
	{
		static void Main(string[] args)
		{
			var botTokenKey = "botToken";
			var botToken = ConfigurationManager.AppSettings.Get(botTokenKey);

			SlackBotV3 slackBot = new SlackBotV3(botToken);
			slackBot.Connect();

			Console.ReadLine();
		}
	}

	static class StringUtility
	{
		private static Regex m_multiSpaceRegex = new Regex(@"\s+", RegexOptions.Compiled);

		public static string NormalizeSpace(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return "";

			s = m_multiSpaceRegex.Replace(s.Trim(), " ");

			return s;
		}
	};

	static class UserUtility
	{
		public static string DisplayName(this User user)
		{
			if (!string.IsNullOrWhiteSpace(user.profile.real_name))
				return user.profile.real_name;

			return user.name;
		}
	}
}
