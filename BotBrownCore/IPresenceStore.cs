namespace BotBrown
{
    using BotBrown.Configuration;

    public interface IPresenceStore
    {
        bool IsSayByeNecessary(ChannelUser user);

        void RemovePresence(ChannelUser user);

        bool IsGreetingNecessary(ChannelUser user);

        void RecordPresence(ChannelUser user);
    }
}