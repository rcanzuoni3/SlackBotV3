namespace SlackBotV3
{
	public interface ICommandHandler
	{
		bool Execute(SlackBotCommand command);
	}
}