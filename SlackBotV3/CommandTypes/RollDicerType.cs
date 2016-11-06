using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	class RollDicerType : CommandType
	{
		public override List<string> CommandNames() { return new List<string>() { "dice" }; }
		public override string Help(string commandName) { return "Type dice to dice the rolls"; }
		public override PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
		public override CommandScope GetCommandScope() { return CommandScope.Global; }

		public override Type GetCommandHandlerType()
		{
			return typeof(RollDicer);
		}

		class RollDicer : CommandHandler
		{
			public RollDicer(SlackBotV3 bot) : base(bot) { }

			public override bool Execute(SlackBotCommand command)
			{
				Random random = new Random();
				int slicesX = random.Next(5) + 1;
				int slicesY = random.Next(5) + 1;
				int slicesZ = random.Next(5) + 1;

				int pieces = slicesX * slicesY * slicesZ;

				SlackBot.Reply(command, string.Format("{0} diced the rolls into {1} pieces", command.User.name, pieces), iconUrl: "https://upload.wikimedia.org/wikipedia/commons/2/28/13-08-31-wien-redaktionstreffen-EuT-by-Bi-frie-134.jpg");

				return false;
			}
		}
	}
}
