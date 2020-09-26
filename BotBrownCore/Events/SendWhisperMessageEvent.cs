namespace BotBrown.Events
{
    using BotBrown;

    public class SendWhisperMessageRequestedEvent : UserSpecificEvent
    {
        public SendWhisperMessageRequestedEvent(ChannelUser user, string message)
            : base(user)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
