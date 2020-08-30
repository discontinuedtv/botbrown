using BotBrownCore.Configuration;

namespace BotBrownCore.Events
{
    public sealed class NewSubscriberEvent : UserSpecificEvent
    {
        public NewSubscriberEvent(ChannelUser user) 
            : base(user)
        {
        }
    }
}
