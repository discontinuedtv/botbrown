namespace BotBrown.Events
{
    using BotBrown;

    public sealed class NewSubscriberEvent : UserSpecificEvent
    {
        public NewSubscriberEvent(ChannelUser user) 
            : base(user)
        {
        }
    }
}
