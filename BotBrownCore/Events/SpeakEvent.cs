namespace BotBrown.Events
{
    using BotBrown;

    public class SpeakEvent : TextToSpeechEvent
    {
        public SpeakEvent(ChannelUser user, string message)
            : base(user, message)
        {
        }
    }
}
