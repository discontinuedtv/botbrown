namespace BotBrown.Events
{
    using BotBrown;

    public abstract class UserSpecificEvent : Event
    {
        protected UserSpecificEvent(ChannelUser user)
        {
            User = user;
        }

        public ChannelUser User { get; }
    }
}
