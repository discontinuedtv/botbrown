namespace BotBrown.Events
{
    using BotBrown;

    public class CommunitySubscriptionEvent : UserSpecificEvent
    {
        public CommunitySubscriptionEvent(ChannelUser user, int numberOfSubscriptionsGifted)
            : base(user)
        {
            NumberOfSubscriptionsGifted = numberOfSubscriptionsGifted;
        }

        public int NumberOfSubscriptionsGifted { get; }
    }
}
