using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SlackBotV3.CommandHandlers
{
	public class FeatureRequestType : ICommandType
	{
		ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "feature", "featureRequest", "featurerequest", "featureList", "fl" }; }
		public string Help(string commandName) { return "Keep a log of the SlackBot feature requests"; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public CommandScope GetCommandScope() { return CommandScope.Global; }
		public Type GetCommandHandlerType() { return typeof(FeatureRequest); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }

		public FeatureRequestType() : this(new CommandHandlerProvider()) { }

		public FeatureRequestType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}
	class FeatureRequest : ICommandHandler
	{
		private SlackBotV3 slackBot;

		public FeatureRequest(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		private const string RequestListFileName = "FeatureRequests.csv";
		private static readonly Encoding encoding = Encoding.ASCII;

		private static readonly string[] ResponseStrings =
		{
				"Great idea! I'll start {0} right away!",
				"OMG! I was just about to suggest {0}. Consider it done.",
				"Yeah. I'll totally begin {0}.",
				"No. {0} is a bad idea.",
				"O.K. {0} is now #1 in the feature queue!",
				"Oh right, {0} could be good. Or we could just not do that.",
				"We just need Edmond to approve {0}, and then we can add that to the queue.",
				"{0}. I remember my first feature request...",
				"I already did that! Just run the command: @{0}",
				"Now how in the Hell am I supposed to add {0} to the queue.",
				"You didn't say the magic words. I refuse to add {0} to the queue.",
				"You know what, I just might implement {0}.",
				"I've been reviewing feature requests since before you were born, and I've never seen an idea as bad as {0}.",
				"Oh yeah I'll begin {0}. It's not like I have anything else on my plate. It's really no problem.",
				"You know who would love to start {0}? Me neither."
			};

		public bool Execute(SlackBotCommand command)
		{
			if (!(command.Channel.name == "the-bureau-of-anarchy" || command.Channel.name == "slackbot-test" || command.Channel.name == "the-anarchy"))
				return true;

			return (command.Name == "featureList" || command.Name == "fl") ? ReadFeatures(command) : WriteFeature(command);
		}

		private bool ReadFeatures(SlackBotCommand command)
		{
			int noOfLines = 0;
			bool returnStatus = false;
			string newline = "\n";//Environment.NewLine;??
			int charSize = encoding.IsSingleByte ? 1 : 2;
			byte[] buffer = null;
			bool printed = false;
			string temp = string.Empty;

			FileStream stream = null;
			try
			{
				stream = new FileStream(GetRelFilePath(RequestListFileName), FileMode.Open,
					FileAccess.Read, FileShare.Write);
				long endPos = stream.Length / charSize, oldPos = 0;
				long posLength;
				printed = false;
				noOfLines = 0;
				buffer = new byte[charSize];
				endPos = stream.Length / charSize;
				if (endPos <= oldPos) oldPos = endPos;      // if file's content is 
															//deleted, reset position
				posLength = endPos - oldPos;

				for (long pos = charSize; pos <= posLength; pos += charSize)
				{
					stream.Seek(-pos, SeekOrigin.End);
					stream.Read(buffer, 0, charSize);
					temp = encoding.GetString(buffer);
					if (temp == newline)
					{
						noOfLines++;
					}
					if (noOfLines == 11)
					{
						buffer = new byte[endPos - stream.Position];
						stream.Read(buffer, 0, buffer.Length);
						slackBot.Reply(command, FormatRequestList(encoding.GetString(buffer)));
						printed = true;
						oldPos = endPos;
						break;
					}
				}
				if (!printed)
				{
					buffer = new byte[endPos - oldPos];
					stream.Seek(-1, SeekOrigin.Current);
					stream.Read(buffer, 0, buffer.Length);
					slackBot.Reply(command, FormatRequestList(encoding.GetString(buffer)));
					oldPos = endPos;
				}
			}
			catch (Exception)
			{
				slackBot.Reply(command, "Whoops, someone did something with the feature list file :(");
				returnStatus = true;
			}
			finally
			{
				if (stream != null)
					stream.Close();
			}

			return returnStatus;
		}

		private string FormatRequestList(string requestList)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string s in requestList.Split('\n'))
			{
				string[] temp = s.Split(',');
				if (temp.Length >= 3)
				{
					if (temp.Length > 3)
					{
						string[] temp2 = new string[temp.Length - 2];
						Array.Copy(temp, 2, temp2, 0, temp2.Length);
						temp[2] = String.Join(",", temp2);
					}
					sb.AppendLine(String.Format("At {0} {1} suggested: {2}", temp[0], temp[1], temp[2]));
				}
			}
			return sb.ToString();
		}

		private bool WriteFeature(SlackBotCommand command)
		{
			Random random = new Random();
			int responseNum = random.Next(ResponseStrings.Length);

			try
			{
				using (StreamWriter sw = File.AppendText(GetRelFilePath(RequestListFileName)))
				{
					sw.WriteLine(DateTime.Now + "," + command.User.name + "," + command.Text);
				}
			}
			catch (Exception)
			{
				slackBot.Reply(command, "Whoops, someone did something with the feature list file :(");
				return true;
			}

			slackBot.Reply(command, String.Format(ResponseStrings[responseNum], command.Text));
			return false;
		}

		private string GetRelFilePath(string fileName)
		{
			while (!File.Exists(fileName))
				fileName = "../" + fileName;

			return fileName;
		}
	}
}
