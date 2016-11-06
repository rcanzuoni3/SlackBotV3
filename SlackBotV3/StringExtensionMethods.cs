using System.Text.RegularExpressions;

namespace SlackBotV3
{
	public static class StringExtensionMethods
	{
		private static Regex multiSpaceRegex = new Regex(@"\s+", RegexOptions.Compiled);

		public static string NormalizeSpace(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return "";

			return multiSpaceRegex.Replace(s.Trim(), " ");
		}
	}
}