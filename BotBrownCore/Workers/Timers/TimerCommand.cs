using System;
using System.Collections.Generic;

namespace BotBrown.Workers.Timers
{
    public sealed class TimerCommand
    {
        private ITimeProvider timeProvider;
        private DateTime doneAt;
        private bool isFinished;

        public TimerCommand(string timerName, DateTime doneAt, ITimeProvider timeProvider)
        {
            this.timeProvider = timeProvider;
            Name = timerName;
            this.doneAt = doneAt;
            isFinished = false;
        }

        public string Name { get; }

        public TimeSpan TimeLeft
        {
            get
            {
                return doneAt.Subtract(timeProvider.Now);
            }
        }

        public string FormattedTimeLeft
        {
            get
            {
                List<string> parts = new List<string>();
                TimeSpan timeLeft = TimeLeft;

                if (timeLeft.Hours > 0)
                {
                    parts.Add($"{timeLeft.Hours} Std");
                }

                if (timeLeft.Minutes > 0)
                {
                    parts.Add($"{timeLeft.Minutes} Min");
                }

                if (timeLeft.Seconds > 0)
                {
                    parts.Add($"{timeLeft.Seconds} Sek");
                }

                return string.Join(" ", parts);
            }
        }

        public bool IsRunning => !isFinished;

        public void Done()
        {
            isFinished = true;
        }
    }
}