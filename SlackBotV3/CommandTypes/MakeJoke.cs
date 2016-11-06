using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace SlackBotV3.CommandTypes
{
	public class MakeJokeType : ICommandType
	{
		private ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "makejoke" }; }
		public string Help(string commandName) { return "Type makejoke followed by someones name"; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public CommandScope GetCommandScope() { return CommandScope.Global; }
		public Type GetCommandHandlerType() { return typeof(MakeJoke); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }

		public MakeJokeType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}

	public class MakeJoke : ICommandHandler
	{
		private SlackBotV3 slackBot;

		public MakeJoke(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		public bool Execute(SlackBotCommand command)
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
					slackBot.Reply(command, HttpUtility.HtmlDecode(joke));
				}
				else
					slackBot.Reply(command, "Crap. I forgot to handle that case.");

				return false;
			}
			catch (Exception)
			{
				slackBot.Reply(command, "Stop trying to break slackbot");
				return false;
			}
		}
	}
}