using System.Collections.Generic;

namespace SlackBotV3
{
	public interface ICommandType
	{
		List<string> CommandNames();
		string Help(string commandName);
		PrivilegeLevel GetPrivilegeLevel();
		CommandScope GetCommandScope();
		ICommandHandler MakeCommandHandler(SlackBotV3 slackBot);
	}
}
