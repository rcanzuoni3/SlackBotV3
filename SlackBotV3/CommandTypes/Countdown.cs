using System;
using System.Collections.Generic;
using System.Threading;

namespace SlackBotV3.CommandTypes
{
	public class CountdownType : ICommandType
	{
		private ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "countdown" }; }
		public string Help(string commandName) { return "Type command followed a number of minutes or amount of time formatted like 00:00:00."; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public CommandScope GetCommandScope() { return CommandScope.Channel; }
		public Type GetCommandHandlerType() { return typeof(Countdown); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }

		public CountdownType() : this(new CommandHandlerProvider()) { }

		public CountdownType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}

	}

	public class Countdown : ICommandHandler
	{
		private class Parameter
		{
			public SlackBotCommand command;
			public Parameter(SlackBotCommand comm)
			{
				command = comm;
			}
		}

		private const double HOUR_IN_MILLISECONDS = 3600000;
		private const double MINUTE_IN_MILLISECONDS = 60000;
		private const double SECOND_IN_MILLISECONDS = 1000;
		private static readonly double[] DISPLAY_TIMES = { HOUR_IN_MILLISECONDS, 30 * MINUTE_IN_MILLISECONDS, 10 * MINUTE_IN_MILLISECONDS, MINUTE_IN_MILLISECONDS, 3 * SECOND_IN_MILLISECONDS, 2 * SECOND_IN_MILLISECONDS, SECOND_IN_MILLISECONDS };
		private static Dictionary<string, double> purposes = new Dictionary<string, double>();
		private string _purpose;
		private string _message;
		private SlackBotV3 slackBot;

		public Countdown(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		private bool Initialize(SlackBotCommand command)
		{
			string[] waitTimeAndPurpose = command.Text.Split(' ');
			string waitTime = waitTimeAndPurpose[0];

			string purpose = "";
			for (int i = 1; i < waitTimeAndPurpose.Length; i++)
				purpose += " " + waitTimeAndPurpose[i];

			bool goodFormat;
			double timeInMinutes = 0;
			if (waitTime.Contains(":"))
			{
				string[] timeComponents = waitTime.Split(':');
				if (timeComponents.Length != 3)
					goodFormat = false;
				else
				{
					int hours, minutes, seconds;
					goodFormat = true;
					goodFormat &= int.TryParse(timeComponents[0], out hours);
					goodFormat &= int.TryParse(timeComponents[1], out minutes) && minutes < 60;
					goodFormat &= int.TryParse(timeComponents[2], out seconds) && seconds < 60;
					if (goodFormat)
						timeInMinutes = ((double)(hours * 3600 + minutes * 60 + seconds)) / 60;
				}
			}
			else
				goodFormat = double.TryParse(waitTime, out timeInMinutes);

			if (!goodFormat)
			{
				_message = "Failed to parse countdown time :( pls don't do bad format";
				return true;
			}

			DateTime startTime = DateTime.Now, endTime = startTime.AddMinutes(timeInMinutes);
			if (endTime.Hour > 18 || startTime.Day != endTime.Day || startTime.Month != endTime.Month || startTime.Year != endTime.Year || timeInMinutes <= 0)
			{
				_message = "Ain't nobody got time for that!";
				return true;
			}

			lock (purposes)
			{
				if (purposes.ContainsKey(purpose))
				{
					this._purpose = purpose;
					_message = "Already waiting for" + purpose + "!\n" + FormatTimeRemaining(purposes[purpose]);
					return true;
				}
				else
				{
					this._purpose = purpose;
					purposes.Add(purpose, Math.Floor(timeInMinutes * MINUTE_IN_MILLISECONDS));
					_message = FormatTimeRemaining(purposes[purpose]);
					return false;
				}
			}
		}

		private bool WaitForTimePeriod(double totalTimeToWait, SlackBotCommand command)
		{
			_message = null;
			foreach (double displayTime in DISPLAY_TIMES)
				if (totalTimeToWait <= displayTime && totalTimeToWait > (displayTime - SECOND_IN_MILLISECONDS))
					_message = FormatTimeRemaining(totalTimeToWait);

			if (_message != null)
				slackBot.Reply(command, _message);

			lock (purposes)
				purposes[_purpose] = totalTimeToWait - SECOND_IN_MILLISECONDS;

			Thread.Sleep((int)SECOND_IN_MILLISECONDS);

			if (totalTimeToWait <= 0)
			{
				lock (purposes)
					purposes.Remove(_purpose);

				return false;
			}
			
			return true;
		}

		private void RunProcess(object parameters)
		{
			SlackBotCommand command = ((Parameter)parameters).command;
			bool keepGoing = true;
			while (keepGoing)
			{
				double time;
				lock (purposes)
					time = purposes[_purpose];

				keepGoing = WaitForTimePeriod(time, command);
			}
			slackBot.Reply(command, ":shark: TIME'S UP :shark:\n" + _purpose.ToUpper() + "!!!!!");
		}

		public bool Execute(SlackBotCommand command)
		{
			bool initializeFailed = Initialize(command);
			slackBot.Reply(command, _message);
			if (initializeFailed)
				return true;

			Thread processThread = new Thread(new ParameterizedThreadStart(RunProcess));
			processThread.Start(new Parameter(command));

			return false;
		}

		private string FormatTimeRemaining(double time)
		{
			double hoursLeft = Math.Floor(time / HOUR_IN_MILLISECONDS);
			double minutesLeft = Math.Floor((time - (hoursLeft * HOUR_IN_MILLISECONDS)) / MINUTE_IN_MILLISECONDS);
			double secondsLeft = (time - (hoursLeft * HOUR_IN_MILLISECONDS) - (minutesLeft * MINUTE_IN_MILLISECONDS)) / SECOND_IN_MILLISECONDS;
			return string.Format("{0} hour{1}, {2} minute{3}, and {4} second{5} remain{6} until{7}...", hoursLeft, hoursLeft == 1 ? "" : "s", minutesLeft, minutesLeft == 1 ? "" : "s", secondsLeft, secondsLeft == 1 ? "" : "s", secondsLeft == 1 ? "s" : "", _purpose);
		}
	}
}