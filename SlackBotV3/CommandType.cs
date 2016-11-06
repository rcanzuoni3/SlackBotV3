using System;
using System.Collections.Generic;
using System.Reflection;

namespace SlackBotV3
{
	abstract class CommandType : ICommandType
	{
		public abstract Type GetCommandHandlerType();

		public abstract List<string> CommandNames();

		public abstract string Help(string commandName);

		public abstract PrivilegeLevel GetPrivilegeLevel();

		public abstract CommandScope GetCommandScope();

		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot)
		{
			Type commandHandlerType = GetCommandHandlerType();
			ConstructorInfo constructorInfo = commandHandlerType.GetConstructor(new Type[] { typeof(SlackBotV3) });
			ICommandHandler commandHandler = (ICommandHandler)constructorInfo.Invoke(new object[] { slackBot });
			return commandHandler;
		}
	}
}
