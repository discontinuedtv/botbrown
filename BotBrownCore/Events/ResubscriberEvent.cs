namespace BotBrownCore.Events
{
    using BotBrownCore.Configuration;

    public class ResubscriberEvent : Event
    {
        public ResubscriberEvent(ChannelUser user, int numberOfMonthsSubscribed)
            : base(user)
        {
            NumberOfMonthsSubscribed = numberOfMonthsSubscribed;
        }

        public int NumberOfMonthsSubscribed { get; }
    }
}
