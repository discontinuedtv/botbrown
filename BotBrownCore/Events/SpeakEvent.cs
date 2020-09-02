namespace BotBrown.Events
{
    using BotBrown.Configuration;

    public class SpeakEvent : TextToSpeechEvent
    {
        public SpeakEvent(ChannelUser user, string message)
            : base(user, message)
        {
        }
    }
}
