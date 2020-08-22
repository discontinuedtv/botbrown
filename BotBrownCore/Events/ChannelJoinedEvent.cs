namespace BotBrownCore.Events
{
    public class ChannelJoinedEvent
    {
        public string ChannelName { get; }

        public ChannelJoinedEvent(string channelName)
        {
            ChannelName = channelName;
        }
    }
}
