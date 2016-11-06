using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("channel_joined")]
    public class ChannelJoined : SlackSocketMessage
    {
        public Channel channel;
    }
}
