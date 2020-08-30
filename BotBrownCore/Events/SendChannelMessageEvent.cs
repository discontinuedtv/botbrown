namespace BotBrownCore.Events
{
    public class SendChannelMessageRequestedEvent : Event
    {
        public SendChannelMessageRequestedEvent(string message, string channelName)
        {
            Message = message;
            ChannelName = channelName;
        }

        public string Message { get; }

        public string ChannelName { get; }
    }
}
