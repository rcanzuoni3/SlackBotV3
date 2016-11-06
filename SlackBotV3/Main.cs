using System.Text.RegularExpressions;

using SlackAPI;
using Ninject;
using System.Reflection;

namespace SlackBotV3
{
	class Run
	{
		static void Main(string[] args)
		{
			var kernel = new StandardKernel();
			kernel.Load(Assembly.GetExecutingAssembly());

			kernel.Get<Program>().RunProgram();

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
