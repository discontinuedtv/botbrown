namespace BotBrownCore.Events
{
    using BotBrownCore.Configuration;

    public class SpeakEvent : TextToSpeechEvent
    {
        public SpeakEvent(ChannelUser user, string message)
            : base(user, message)
        {
        }
    }
}
