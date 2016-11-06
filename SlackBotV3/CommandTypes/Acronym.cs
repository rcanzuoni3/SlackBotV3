using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace SlackBotV3.CommandTypes
{
	public class AcronymType : ICommandType
	{
		private ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "acronym" }; }
		public string Help(string commandName) { return "Make an acronym out of some words"; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public CommandScope GetCommandScope() { return CommandScope.Global; }
		public Type GetCommandHandlerType() { return typeof(Acronym); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }

		public AcronymType() : this(new CommandHandlerProvider()) { }

		public AcronymType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}

	public class Acronym : ICommandHandler
	{
		public Acronym(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		private SlackBotV3 slackBot;

		public bool Execute(SlackBotCommand command)
		{
			string output = "";
			foreach (string part in command.Text.Split(' '))
			{
				string searchTerm = part.NormalizeSpace();
				if (searchTerm.Length >= 3 && searchTerm.Length <= 15)
				{
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("http://acronym-maker.com/generate/?w={0}&wl=", System.Uri.EscapeDataString(searchTerm)));
					HttpWebResponse response = (HttpWebResponse)request.GetResponse();
					StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
					string xml = reader.ReadToEnd();
					response.Dispose();

					XmlDocument doc = new XmlDocument();
					if (request.Address.ToString().Contains("index.php"))
					{
						slackBot.Reply(command, "You dun sumthin real bad with them thar characters...");
						return true;
					}

					doc.LoadXml(xml.Substring(xml.IndexOf("<table"), xml.IndexOf("</table>") - xml.IndexOf("<table")) + "</table>");
					XmlNode documentNode = doc.DocumentElement;
					try
					{
						foreach (XmlNode acronymRow in documentNode.SelectNodes("//*[local-name() = 'table']//*[local-name()='tr']"))
							output += acronymRow.SelectSingleNode(".//*[local-name()='th']").InnerText.Trim() + ": " +
									  acronymRow.SelectSingleNode(".//*[local-name()='a' or local-name()='td']").InnerText.Trim() + "\n";
					}
					catch (Exception e)
					{
						slackBot.Reply(command, e.Message);
						return true;
					}
					output += "\n";
				}
				else
				{
					slackBot.Reply(command, String.Format("The word has to be 3-15 characters. Violator: \"{0}\"", searchTerm));
					return true;
				}
			}
			slackBot.Reply(command, output);
			return false;
		}
	}
}
