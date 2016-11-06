using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SlackBotV3.CommandTypes
{
	class GoodJobType : CommandType
	{
		public override List<string> CommandNames() { return new List<string>() { "goodjob", "badjob", "gj", "bj", "checkjobs" }; }
		public override string Help(string commandName) { return "Type goodjob or badjob followed by a user's name and a description of the job"; }
		public override PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public override CommandScope GetCommandScope() { return CommandScope.Global; }

		public override Type GetCommandHandlerType()
		{
			return typeof(GoodJob);
		}

		class GoodJob : CommandHandler
		{
			public GoodJob(SlackBotV3 bot) : base(bot) { }

			private const int GoodJobIndex = 0;
			private const int BadJobIndex = 1;

			private const string JobMatrixFileName = "Jobs.txt";

			private static readonly string[] Responses =
			{
				"I never thought you'd get so many jobs.",
				"Work will set you free!",
				"Try to catch 'em all!",
				"Step yo game up to the streets.",
				"You can get more from doing things irl.",
				":shark: is displeased with you."
			};

			private static Dictionary<string, int[]> JobMatrix = ReadJobMatrix(GetRelFilePath(JobMatrixFileName));

			public override bool Execute(SlackBotCommand command)
			{
				if (!(command.Channel.name == "the-bureau-of-anarchy" || command.Channel.name == "slackbot-test" || command.Channel.name == "the-anarchy"))
					return false;

				string[] commandSplit = command.Text.Split(' ');

				string username = "";
				if (commandSplit.Length >= 1 && !String.IsNullOrEmpty(commandSplit[0]))
				{
					username = GetUsername(command, commandSplit[0]);
					if (username == null)
					{
						SlackBot.Reply(command, String.Format("You messed up, {0}. You get a bad job.", command.User.name));
						if (!JobMatrix.ContainsKey(command.User.name))
							JobMatrix.Add(command.User.name, new[] { 0, 0 });

						JobMatrix[command.User.name][BadJobIndex] ++;
						WriteJobMatrix(GetRelFilePath(JobMatrixFileName));
						return false;
					}
				}

				if (command.Name == "checkjobs")
				{
					Random ran = new Random();
					string userToLookup = string.IsNullOrWhiteSpace(username) ? command.User.name : username;
					if (JobMatrix.ContainsKey(userToLookup))
						SlackBot.Reply(command, String.Format("{2}\nGood jobs: {0}\nBad jobs:{1}\n{3}", FormatJobs(JobMatrix[userToLookup][GoodJobIndex], GoodJobIndex), FormatJobs(JobMatrix[userToLookup][BadJobIndex], BadJobIndex), userToLookup, Responses[ran.Next(Responses.Length)]));
					else
						SlackBot.Reply(command, "No jobs yet, get working!");
					return false;
				}

				if (!JobMatrix.ContainsKey(username))
					JobMatrix.Add(username, new []{ 0, 0 });

				bool goodJob;
				if (command.Name == "goodjob" || command.Name == "gj")
				{
					JobMatrix[username][GoodJobIndex] ++;
					goodJob = true;
				}
				else
				{
					JobMatrix[username][BadJobIndex] ++;
					goodJob = false;
				}
				if (!JobMatrix.ContainsKey(command.User.name))
					JobMatrix.Add(command.User.name, new []{ 0, 0 });

				JobMatrix[command.User.name][GoodJobIndex] ++;

				WriteJobMatrix(GetRelFilePath(JobMatrixFileName));

				SlackBot.Reply(command, String.Format("You've given a {0} to {1} for {2}. Good job on giving out jobs.", goodJob ? "good job" : "bad job", username, GetJobDescription(commandSplit)));
				return false;
			}

			private string GetUsername(SlackBotCommand command, string name)
			{
				string opponentId;
				if (name.StartsWith("<@") && name.EndsWith(">"))
				{
					opponentId = name.Substring(2, name.Length - 3);
					if (Array.Find(command.Channel.members, userId => opponentId == userId) == null)
						return null;
				}
				else
				{
					opponentId = Array.Find(command.Channel.members,
						userId => SlackBot.GetUser(userId) != null && SlackBot.GetUser(userId).name == name);
					if (opponentId == null)
						return null;
				}
				return SlackBot.GetUser(opponentId).name;
			}

			private string GetJobDescription(string[] commandSplit)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 1; i < commandSplit.Length; i++)
				{
					sb.Append(commandSplit[i]);
					if (i + 1 < commandSplit.Length)
						sb.Append(" ");
				}
				return sb.ToString();
			}

			private string FormatJobs(int numJobs, int jobType)
			{
				StringBuilder jobHundreds = new StringBuilder();
				StringBuilder jobTens = new StringBuilder();
				StringBuilder jobOnes = new StringBuilder();

				int numHundreds = numJobs/100;
				for (int i = 0; i < numHundreds; i++)
					jobHundreds.Append("{0}");

				int numTens = (numJobs - numHundreds*100)/10;
				for (int i = 0; i < numTens; i++)
					jobTens.Append("{0}");

				int numOnes = ((numJobs - numHundreds*100) - numTens*10);
				for (int i = 0; i < numOnes; i++)
					jobOnes.Append("{0}");

				return String.Format(jobHundreds.ToString(), jobType == GoodJobIndex ? ":100:" : ":poop:") +
					String.Format(jobTens.ToString(), jobType == GoodJobIndex ? ":star2:" : ":fu:") + 
					String.Format(jobOnes.ToString(), jobType == GoodJobIndex ? ":star:" : ":x:");
			}

			private static void WriteJobMatrix(string file)
			{
				if (File.Exists(file))
					File.Delete(file);

				using (StreamWriter sw = new StreamWriter(file))
				{
					foreach (string user in JobMatrix.Keys)
					{
						sw.WriteLine("{0};{1},{2}", user, JobMatrix[user][GoodJobIndex], JobMatrix[user][BadJobIndex]);
					}
					sw.Close();
				}
			}

			private static Dictionary<string, int[]> ReadJobMatrix(string file)
			{
				if (!File.Exists(file))
					return new Dictionary<string, int[]>();
				
				Dictionary<string, int[]> matrix = new Dictionary<string, int[]>();
				using (StreamReader sr = new StreamReader(file))
				{
					while (!sr.EndOfStream)
					{
						string line = sr.ReadLine();
						if (line == null)
							continue;

						string[] splitLine = line.Split(';');
						string[] jobs = splitLine[1].Split(',');
						matrix.Add(splitLine[0], new[] {int.Parse(jobs[GoodJobIndex]), int.Parse(jobs[BadJobIndex])});
					}
					sr.Close();
				}
				return matrix;
			}

			private static string GetRelFilePath(string fileName)
			{
				while (!File.Exists(fileName))
					fileName = "../" + fileName;

				return fileName;
			}
		}
	}
}