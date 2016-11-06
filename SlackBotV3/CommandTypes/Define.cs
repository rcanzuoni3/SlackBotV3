using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	class DefineType : CommandType
    {
        public override List<string> CommandNames() { return new List<string>() { "define" }; }
        public override string Help(string commandName) { return "Type define followed by phrase to lookup defintion"; }
        public override PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
        public override CommandScope GetCommandScope() { return CommandScope.Global; }

        public override Type GetCommandHandlerType()
        {
            return typeof(Define);
        }

        class Define : CommandHandler
        {
            public Define(SlackBotV3 bot) : base(bot) { }

            public override bool Execute(SlackBotCommand command)
            {
                SlackBot.Reply(command, "http://www.urbandictionary.com/define.php?term=" + System.Uri.EscapeDataString(command.Text));
                return false;
            }
        }
    }
}