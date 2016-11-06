using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    [RequestPath("channels.invite")]
    public class ChannelsInviteResponse : MessageHistory
    {
        Channel channel;
        OwnedStampedMessage purpose;
    }
}
