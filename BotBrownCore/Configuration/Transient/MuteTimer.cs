namespace BotBrown.Configuration.Transient
{
    using System;

    public class MuteTimer
    {
        private readonly int muteDuration;
        private readonly DateTimeOffset muteSince;

        public MuteTimer(int muteDuration = 10)
        {
            this.muteDuration = muteDuration;
            muteSince = DateTimeOffset.Now;
        }

        public bool IsMute()
        {
            return muteSince.AddMinutes(muteDuration) > DateTimeOffset.Now;
        }
    }
}
