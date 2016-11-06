using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
	class LooookIsABigOlDummyType : CommandType
    {
        public override List<string> CommandNames() { return new List<string>() { "lookDumb" }; }
        public override string Help(string commandName) { return "Type to mock loooook"; }
        public override PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
        public override CommandScope GetCommandScope() { return CommandScope.Global; }

        public override Type GetCommandHandlerType()
        {
            return typeof(LooookIsABigOlDummy);
        }

        class LooookIsABigOlDummy : CommandHandler
        {
            public LooookIsABigOlDummy(SlackBotV3 bot) : base(bot) { }

            public override bool Execute(SlackBotCommand command)
            {
                SlackBot.Reply(command, string.Format("Looooook is a big ol' Dummy"), iconUrl: "https://media.licdn.com/mpr/mpr/shrinknp_400_400/p/5/005/06f/355/12ceec5.jpg");
                return false;
            }
        }
    }
}
