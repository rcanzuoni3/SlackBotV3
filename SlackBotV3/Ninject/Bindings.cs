using Ninject.Modules;

namespace SlackBotV3.Ninject
{
	public class Bindings : NinjectModule
	{
		public override void Load()
		{
			Bind<IBotTokenProvider>().To<BotTokenProvider>();
		}
	}
}
