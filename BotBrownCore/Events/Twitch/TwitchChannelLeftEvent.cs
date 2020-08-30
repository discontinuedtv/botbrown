namespace BotBrownCore.Events.Twitch
{
    public sealed class TwitchChannelLeftEvent : Event
    {
        public TwitchChannelLeftEvent(string channelName)
        {
            ChannelName = channelName;
        }

        public string ChannelName { get; }
    }
}
