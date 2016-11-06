using System;
using System.Collections.Generic;
using System.IO;

namespace SlackBotV3.CommandTypes
{
	class KonamiCodeType : CommandType
	{
		public override List<string> CommandNames() { return new List<string>() { "up_up_down_down_left_right_left_right_b_a" }; }
		public override string Help(string commandName) { return "The Konami Code!!!"; }
		public override PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Super; }
		public override CommandScope GetCommandScope() { return CommandScope.Global; }

		public override Type GetCommandHandlerType()
		{
			return typeof(KonamiCode);
		}

		class KonamiCode : CommandHandler
		{
			public KonamiCode(SlackBotV3 bot) : base(bot) { }
			private const int GoodJobIndex = 0;
			private const int BadJobIndex = 1;
			private const string JobMatrixFileName = "Jobs.txt";
			private static Dictionary<string, int[]> JobMatrix = ReadJobMatrix(GetRelFilePath(JobMatrixFileName));
			public override bool Execute(SlackBotCommand command)
			{
				/*string breakBot = null;
				if (breakBot.Length > 9001)
					SlackBot.Reply(command, "how did I get here?");
				*/
				if (!JobMatrix.ContainsKey(command.User.name))
					return false;

				JobMatrix[command.User.name][GoodJobIndex] += 100;
				WriteJobMatrix(GetRelFilePath(JobMatrixFileName));
				return false;
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
						matrix.Add(splitLine[0], new[] { int.Parse(jobs[GoodJobIndex]), int.Parse(jobs[BadJobIndex]) });
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
		}

	}
}
