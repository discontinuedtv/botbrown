namespace BotBrownCore.Events
{
    using BotBrownCore.Configuration;

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
