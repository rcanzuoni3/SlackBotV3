using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandHandlers
{
	class DiceRollerType : CommandType
    {
        public override List<string> CommandNames(){ return new List<string>(){"roll"}; }
        public override string Help(string commandName) { return "Type roll to roll the dice";  }
        public override PrivilegeLevel GetPrivilegeLevel(){ return PrivilegeLevel.Normal; }
        public override CommandScope GetCommandScope() { return CommandScope.Global; }

        public override Type GetCommandHandlerType()
        {
            return typeof(DiceRoller);
        }

        class DiceRoller : CommandHandler
        {
            public DiceRoller(SlackBotV3 bot) : base(bot) { }

            public override bool Execute(SlackBotCommand command)
            {
                Random random = new Random();
                int die1 = random.Next(6) + 1;
                int die2 = random.Next(6) + 1;

                int roll = die1 + die2;

                SlackBot.Reply(command, string.Format("{0} rolled a {1}", command.User.name, roll), iconUrl: "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c4/2-Dice-Icon.svg/2000px-2-Dice-Icon.svg.png");

                return false;
            }
        }
    }
}
