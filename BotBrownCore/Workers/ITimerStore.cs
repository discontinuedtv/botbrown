namespace BotBrown.Workers
{
    using BotBrown.Workers.Timers;
    using System.Collections.Generic;

    public interface ITimerStore
    {
        IEnumerable<TimerCommand> GetAllTimers();

        void DeleteTimer(TimerCommand timer);

        void AddTimer(TimerCommand timer);
    }
}
