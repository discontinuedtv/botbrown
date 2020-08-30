namespace BotBrownCore.Events
{
    using BotBrownCore.Configuration;

    public class ResubscriberEvent : UserSpecificEvent
    {
        public ResubscriberEvent(ChannelUser user, int numberOfMonthsSubscribed)
            : base(user)
        {
            NumberOfMonthsSubscribed = numberOfMonthsSubscribed;
        }

        public int NumberOfMonthsSubscribed { get; }
    }
}
