namespace BotBrownCore.Events
{
    using BotBrownCore.Configuration;

    public abstract class Event
    {
        protected Event(ChannelUser user)
        {
            User = user;
        }

        public ChannelUser User { get; }
    }
}
