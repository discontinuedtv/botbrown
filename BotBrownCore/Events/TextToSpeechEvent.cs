namespace BotBrown.Events
{
    using BotBrown.Configuration;

    public class TextToSpeechEvent : UserSpecificEvent
    {
        public TextToSpeechEvent(ChannelUser user, string message) 
            : base(user)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
