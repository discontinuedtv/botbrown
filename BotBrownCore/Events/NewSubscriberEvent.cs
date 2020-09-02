using BotBrown.Configuration;

namespace BotBrown.Events
{
    public sealed class NewSubscriberEvent : UserSpecificEvent
    {
        public NewSubscriberEvent(ChannelUser user) 
            : base(user)
        {
        }
    }
}
