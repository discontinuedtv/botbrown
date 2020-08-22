using BotBrownCore.Configuration;

namespace BotBrownCore.Events
{
    public sealed class NewSubscriberEvent : Event
    {
        public NewSubscriberEvent(ChannelUser user) 
            : base(user)
        {
        }
    }
}
