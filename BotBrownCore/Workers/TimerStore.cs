namespace BotBrown.Workers
{
    using BotBrown.Workers.Timers;
    using System.Collections.Generic;

    public class TimerStore : ITimerStore
    {
        private readonly IList<TimerCommand> timers = new List<TimerCommand>();

        public void DeleteTimer(TimerCommand timer)
        {
            timers.Remove(timer);
        }

        public IEnumerable<TimerCommand> GetAllTimers()
        {
            return timers;
        }
    }
}
