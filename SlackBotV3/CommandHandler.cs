namespace SlackBotV3
{
	abstract class CommandHandler : ICommandHandler
    {
        public CommandHandler(SlackBotV3 bot)
        {
            SlackBot = bot;
        }

        public abstract bool Execute(SlackBotCommand command);

        protected SlackBotV3 SlackBot;

    }
}
