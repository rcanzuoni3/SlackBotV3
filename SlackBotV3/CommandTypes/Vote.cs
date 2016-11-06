using System;
using System.Collections.Generic;

namespace SlackBotV3.CommandTypes
{
    class VoteType : CommandType
    {
        public override List<string> CommandNames() { return new List<string>() { "makevote", "vote" }; }
        public override string Help(string commandName) {
            if (commandName == "makevote")
                return "Type makevote followed by a topic to vote on.  Only one vote in a channel at a time."+
                    "The vote ends when a majority of active non-bot users have voted.";
            else
                return "Type vote followed by yes or no.";
        }
        
        public override PrivilegeLevel GetPrivilegeLevel() { return PrivilegeLevel.Normal; }
        public override CommandScope GetCommandScope() { return CommandScope.Channel; }

        public override Type GetCommandHandlerType()
        {
            return typeof(Vote);
        }

        class Vote : CommandHandler
        {
            private HashSet<string> Voted = new HashSet<string>();
            private int YesVotes = 0;
            private int NoVotes = 0;
            private string VotingTopic = null;

            public Vote(SlackBotV3 bot) : base(bot) { }

            public override bool Execute(SlackBotCommand command)
            {
                if(command.Name == "makevote")
                {
                    if (string.IsNullOrWhiteSpace(command.Text))
                    {
                        SlackBot.Reply(command, "What do you want to vote on?");
                        return false;
                    }
                    else if (VotingTopic != null)
                    {
                        SlackBot.Reply(command, "There is already an ongoing vote.");
                        return true;
                    }

                    VotingTopic = command.Text;
                    return true;
                }else
                {
                    if (VotingTopic == null)
                    {
                        SlackBot.Reply(command, "There is no vote going on currently");
                        return false;
                    }
                    if (!(command.Text == "yes" || command.Text == "no"))
                    {
                        SlackBot.Reply(command, "You must vote 'yes' or 'no'");
                        return true;
                    }
                    else if (Voted.Contains(command.User.id))
                    {
                        SlackBot.Reply(command, "You already voted");
                        return true;
                    }

                    if (command.Text == "yes")
                        YesVotes++;
                    else
                        NoVotes++;
                    Voted.Add(command.User.id);

                    int difference = Math.Abs(YesVotes - NoVotes);
                    int votes = YesVotes + NoVotes;
                    int votesLeft = Array.FindAll(command.Channel.members, userId => SlackBot.GetUser(userId).presence == "active" && !SlackBot.GetUser(userId).is_bot).Length - votes;

                    if (difference > votesLeft)
                    {
                        string result = (YesVotes > NoVotes) ? "yes" : "no";
                        SlackBot.Reply(command, "The people have spoken, the vote is " + result + " to " + VotingTopic);
                        return false;
                    }
                    else if (votesLeft == 0)
                    {
                        SlackBot.Reply(command, "The vote was inconclusive.");
                        return false;
                    }

                    return true;
                }
            }
        }
    }
}