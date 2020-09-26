namespace BotBrown.Events
{
    using BotBrown;

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
