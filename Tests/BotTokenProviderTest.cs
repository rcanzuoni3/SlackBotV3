using NUnit.Framework;
using SlackBotV3;
using System.Collections.Generic;

namespace Tests
{
	[TestFixture]
	public class BotTokenProviderTest
	{
		IBotTokenProvider subject;

		[SetUp]
		public void Setup()
		{
			subject = new BotTokenProvider();
		}

		[Test]
		public void GetBotToken_Throws_Key_Not_Found_Exception_When_Key_Is_Not_In_App_Config()
		{
			var keyNotInAppConfig = "ajksdfl;asdflasdfasl;djf";

			Assert.Throws<KeyNotFoundException>(() => subject.GetBotToken(keyNotInAppConfig));
		}
	}
}
