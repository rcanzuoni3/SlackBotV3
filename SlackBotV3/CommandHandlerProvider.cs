using System;

namespace SlackBotV3
{
	public interface ICommandHandlerProvider
	{
		ICommandHandler GetCommandHandler(SlackBotV3 slackBot, Type CommandHandlerType);
	}

	public class CommandHandlerProvider : ICommandHandlerProvider
	{
		public ICommandHandler GetCommandHandler(SlackBotV3 slackBot, Type commandHandlerType)
		{
			var constructorInfo = commandHandlerType.GetConstructor(new Type[] { typeof(SlackBotV3) });
			var commandHandler = (ICommandHandler)constructorInfo.Invoke(new object[] { slackBot });
			return commandHandler;
		}
	}
}
