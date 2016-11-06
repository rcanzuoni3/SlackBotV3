using System;

namespace SlackAPI.WebSocketMessages
{
    [SlackSocketRouting("channel_created")]
    public class ChannelCreated: SlackSocketMessage
    {
        public Channel channel;
    }
}
