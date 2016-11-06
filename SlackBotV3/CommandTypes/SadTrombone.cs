using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	class SadTromboneType : CommandType
    {
        public override List<string> CommandNames() { return new List<string>() { "sadtrombone" }; }
        public override string Help(string commandName) { return "Plays a sad trombone"; }
        public override PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
        public override CommandScope GetCommandScope() { return CommandScope.Global; }

        public override Type GetCommandHandlerType()
        {
            return typeof(SadTrombone);
        }

        class SadTrombone : CommandHandler
        {
            public SadTrombone(SlackBotV3 bot) : base(bot) { }

            public override bool Execute(SlackBotCommand command)
            {
                SlackBot.Reply(command, "http://www.sadtrombone.com/");
                return false;
            }
        }
    }
}
