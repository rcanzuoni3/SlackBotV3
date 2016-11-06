using Ninject.Modules;
using SlackBotV3.CommandHandlers;
using SlackBotV3.CommandTypes;

namespace SlackBotV3.Ninject
{
	public class Bindings : NinjectModule
	{
		public override void Load()
		{
			Bind<IBotTokenProvider>().To<BotTokenProvider>();
			Bind<ICommandHandlerProvider>().To<CommandHandlerProvider>();
			Bind<SlackBotV3>().ToSelf().InSingletonScope();

			Bind<AcronymType>().ToSelf();
			Bind<CheckPrivelegeType>().ToSelf();
			Bind<CountdownType>().ToSelf();
			Bind<DefineType>().ToSelf();
			Bind<DiceRollerType>().ToSelf();
			Bind<EmojifyType>().ToSelf();
			Bind<FeatureRequestType>().ToSelf();
			Bind<GoodJobType>().ToSelf();
			Bind<GoodShitType>().ToSelf();
			Bind<HelpType>().ToSelf();
			Bind<LMGTFYType>().ToSelf();
			Bind<MakeJokeType>().ToSelf();
			Bind<MartiniType>().ToSelf();
			Bind<MockType>().ToSelf();
			Bind<RenameCommandType>().ToSelf();
			Bind<RestartType>().ToSelf();
			Bind<RockPaperScissorsType>().ToSelf();
			Bind<RollDicerType>().ToSelf();
			Bind<SadTromboneType>().ToSelf();
			Bind<TicTacToeType>().ToSelf();
			Bind<VoteType>().ToSelf();
			Bind<WeatherType>().ToSelf();
		}
	}
}
