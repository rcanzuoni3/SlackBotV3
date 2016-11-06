using System;
using System.Collections.Generic;
using System.Text;

namespace SlackBotV3.CommandTypes
{
	class MartiniType : ICommandType
	{
		private ICommandHandlerProvider commandHandlerProvider;

		public List<string> CommandNames() { return new List<string>() { "martini" }; }
		public string Help(string commandName) { return "Makes a martini glass"; }
		public PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public CommandScope GetCommandScope() { return CommandScope.Global; }
		public Type GetCommandHandlerType() { return typeof(Martini); }
		public ICommandHandler MakeCommandHandler(SlackBotV3 slackBot) { return commandHandlerProvider.GetCommandHandler(slackBot, GetCommandHandlerType()); }

		public MartiniType(ICommandHandlerProvider commandHandlerProvider)
		{
			this.commandHandlerProvider = commandHandlerProvider;
		}
	}

	public class Martini : ICommandHandler
	{
		private SlackBotV3 slackBot;

		public Martini(SlackBotV3 slackBot)
		{
			this.slackBot = slackBot;
		}

		public bool Execute(SlackBotCommand command)
		{
			string size = string.Empty;
			int sizeInt = 0;
			try
			{
				if (!int.TryParse(command.Text, out sizeInt))
				{
					slackBot.Reply(command, "You disappoint your family");
					return false;
				}
				if (sizeInt > 33)
				{
					slackBot.Reply(command, "The count is too high. Heck now its " + sizeInt * 9000 + "/9000");
					return false;
				}
				slackBot.Reply(command, printGlass(sizeInt));
			}
			catch (Exception)
			{
				slackBot.Reply(command, "You done gone broke the Nexus");
				return false;
			}

			return false;
		}

		private string printGlass(int size)
		{
			int glassWidth = size * 2 - 1; //The width of the glass can be found from the input
			StringBuilder sb = new StringBuilder();
			sb.Append("```");
			//DRAW CUP PART OF GLASS
			for (int i = 0; i < size; i++)
			{//Height of the glass
				for (int j = 0; j < glassWidth; j++)
				{//Width of the glass
					if (j >= i && j < glassWidth - i)//
						sb.Append("0");
					else
						sb.Append(" ");
				}
				sb.Append("\n"); //new line
			}//end outer loop i

			//DRAW STEM AND BASE
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size - 1; j++)
					sb.Append(" ");
				sb.Append("|\n"); //Draw a stem and new line
								  //					
			}
			for (int i = 0; i < glassWidth; i++)
				sb.Append("="); //Draw base

			sb.Append("```");
			return sb.ToString();
		}
	}
}
