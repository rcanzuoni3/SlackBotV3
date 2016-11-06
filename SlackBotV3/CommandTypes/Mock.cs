using System;
using System.Collections.Generic;

using SlackAPI;
namespace SlackBotV3.CommandTypes
{
	class MockType : CommandType
    {
        public override List<string> CommandNames() { return new List<string>() { "mock" }; }
        public override string Help(string commandName) { return "Type mock followed by the username or real name of the user you want SlackBot to mock."; }
        public override PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
        public override CommandScope GetCommandScope() { return CommandScope.Global; }

        public override Type GetCommandHandlerType()
        {
            return typeof(Mock);
        }

        class Mock : CommandHandler
        {
            public Mock(SlackBotV3 bot) : base(bot) { }

            public override bool Execute(SlackBotCommand command)
            {
                User userToMock;

                if (string.IsNullOrWhiteSpace(command.Text))
                    userToMock = command.User;
                else
                {
                    string userId = command.Text;
                    userToMock = SlackBot.GetUsers().Find(user => user.name == userId);
                    if (userToMock == null)
                        userToMock = SlackBot.GetUsers().Find(user => user.profile.real_name == userId);

                    if (userToMock == null)
                    {
                        userId = userId.Replace("\"", "");
                        userId = "\"" + userId + "\"";
                        SlackBot.Reply(command, string.Format("Sorry {0}, I cannot find user {1}", command.User.name, userId));
                        return false; ;
                    }
                }

                string userName;
                if (!string.IsNullOrWhiteSpace(userToMock.profile.real_name))
                    userName = userToMock.profile.real_name;
                else
                    userName = userToMock.name;

                SlackBot.Reply(command, string.Format("I'm {0} and I'm dumb", userName), userName, userToMock.profile.image_32);
                return false;
            }
        }
    }
}
