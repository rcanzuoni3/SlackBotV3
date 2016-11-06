using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	class LMGTFYType : CommandType
    {
        public override List<string> CommandNames() { return new List<string>() { "lmgtfy" }; }
        public override string Help(string commandName) { return "Let me google that for you"; }
        public override PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
        public override CommandScope GetCommandScope() { return CommandScope.Global; }

        public override Type GetCommandHandlerType()
        {
            return typeof(LMGTFY);
        }

        class LMGTFY : CommandHandler
        {
            public LMGTFY(SlackBotV3 bot) : base(bot) { }

            public override bool Execute(SlackBotCommand command)
            {
                SlackBot.Reply(command, "http://lmgtfy.com/?q=" + System.Uri.EscapeDataString(command.Text));
                return false;
            }
        }
    }
}
