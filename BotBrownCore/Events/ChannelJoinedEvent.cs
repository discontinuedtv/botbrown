namespace BotBrown.Events
{
    public class TwitchChannelJoinedEvent : Event
    {
        public string ChannelName { get; }

        public TwitchChannelJoinedEvent(string channelName)
        {
            ChannelName = channelName;
        }
    }
}
