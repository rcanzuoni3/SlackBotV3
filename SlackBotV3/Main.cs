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
}
