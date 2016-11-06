using System;
using System.Collections.Generic;

namespace SlackBotV3
{
	class CommandHandlerRegistry
    {
        public CommandHandlerRegistry(CommandTypeRegistry commandHandlerRegistry)
        {
            CommandTypeRegistry = commandHandlerRegistry;
        }

        public void RegisterCommand(SlackBotCommand command, SlackBotV3 slackBot)
        {
            ICommandType commandType = CommandTypeRegistry.GetCommandType(command.Name);
            ICommandHandler commandHandler = commandType.MakeCommandHandler(slackBot);

            CommandContainer[GetCommandKey(command)] = commandHandler;
        }

        public void DeRegisterCommand(SlackBotCommand command)
        {
            CommandContainer.Remove(GetCommandKey(command));
        }

        public bool HasCommand(SlackBotCommand command)
        {
            return CommandContainer.ContainsKey(GetCommandKey(command));
        }

        public ICommandHandler GetCommand(SlackBotCommand command)
        {
            return CommandContainer[GetCommandKey(command)];
        }

        private int GetCommandKey(SlackBotCommand command)
        {
            int commandId = CommandTypeRegistry.GetCommandId(command.Name);
            CommandScope scope = CommandTypeRegistry.GetCommandType(command.Name).GetCommandScope();

            object preKey = null;
            switch(scope)
            {
                case CommandScope.Global:
                    preKey = commandId; break;
                case CommandScope.Channel:
                    preKey = new Tuple<int, string>(commandId, command.Channel.id); break;
                case CommandScope.User:
                    preKey = new Tuple<int,string,string>(commandId, command.Channel.id, command.User.id); break;
            }

            return preKey.GetHashCode();
        }

        private CommandTypeRegistry CommandTypeRegistry;

        private Dictionary<int, ICommandHandler> CommandContainer = new Dictionary<int, ICommandHandler>();
    }
}
