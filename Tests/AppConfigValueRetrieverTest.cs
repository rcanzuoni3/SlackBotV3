using NUnit.Framework;
using SlackBotV3;
using System.Collections.Generic;

namespace Tests
{
	[TestFixture]
	public class AppConfigValueRetrieverTest
	{
		IAppConfigValueRetriever subject;

		[SetUp]
		public void Setup()
		{
			subject = new AppConfigValueRetriever();
		}

		[Test]
		public void GetBotToken_Throws_Key_Not_Found_Exception_When_Key_Is_Not_In_App_Config()
		{
			var keyNotInAppConfig = "ajksdfl;asdflasdfasl;djf";

			Assert.Throws<KeyNotFoundException>(() => subject.GetValue(keyNotInAppConfig));
		}
	}
}
