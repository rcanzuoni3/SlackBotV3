using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	class RenameCommandType : CommandType
    {
        public override List<string> CommandNames(){ return new List<string>(){"rename"}; }
        public override string Help(string commandName) { return "Type rename followed by the new bot name";  }
        public override PrivilegeLevel GetPrivilegeLevel(){ return PrivilegeLevel.Admin; }
        public override CommandScope GetCommandScope() { return CommandScope.Global; }

        public override Type GetCommandHandlerType()
        {
            return typeof(Rename);
        }

        class Rename : CommandHandler
        {
            public Rename(SlackBotV3 bot) : base(bot) { }

            public override bool Execute(SlackBotCommand command)
            {
                if (string.IsNullOrWhiteSpace(command.Text))
                    SlackBot.Reply(command, "You did not specify a new name");
                else
                    SlackBot.RenameBot(command.Text);

                return false;
            }
        }
    }
}