using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace SlackBotV3.CommandTypes
{
	class MakeJokeType : CommandType
	{
		public override List<string> CommandNames() { return new List<string>() { "makejoke" }; }
		public override string Help(string commandName) { return "Type makejoke followed by someones name"; }
		public override PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public override CommandScope GetCommandScope() { return CommandScope.Global; }

		public override Type GetCommandHandlerType()
		{
			return typeof(Weather);
		}

		class Weather : CommandHandler
		{
			public Weather(SlackBotV3 bot) : base(bot) { }

			public override bool Execute(SlackBotCommand command)
			{
				try
				{
					string name = string.IsNullOrWhiteSpace(command.Text) ? "The Great Ranzuoni" : command.Text;
					HttpWebRequest request =
						(HttpWebRequest)
							WebRequest.Create("http://api.icndb.com/jokes/random");

					HttpWebResponse response = (HttpWebResponse)request.GetResponse();
					StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
					string json = reader.ReadToEnd();
					response.Dispose();

					JObject joResponse = JObject.Parse(json);
					if (joResponse["value"] != null)
					{
						string joke = joResponse["value"]["joke"].ToString();
						joke = joke.Replace("Chuck Norris", name);
						SlackBot.Reply(command, HttpUtility.HtmlDecode(joke));
					}
					else
						SlackBot.Reply(command, "Crap. I forgot to handle that case.");

					return false;
				}
				catch (Exception)
				{
					SlackBot.Reply(command, "Stop trying to break slackbot");
					return false;
				}
			}
		}
	}
}