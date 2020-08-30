using System;
using System.Collections.Generic;

namespace BotBrownCore.Workers.Timers
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
                    string hourUnit = timeLeft.Hours > 1 ? "Stunden" : "Stunde";
                    parts.Add($"{timeLeft.Hours} {hourUnit}");
                }

                if (timeLeft.Minutes > 0)
                {
                    string minuteUnit = timeLeft.Minutes > 1 ? "Minuten" : "Minute";
                    parts.Add($"{timeLeft.Minutes} {minuteUnit}");
                }

                if (timeLeft.Seconds > 0)
                {
                    string secondUnit = timeLeft.Seconds > 1 ? "Sekunden" : "Sekunde";
                    parts.Add($"{timeLeft.Seconds} {secondUnit}");
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