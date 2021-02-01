using System;

namespace BotBrown.Models
{
    public class Time
    {
        private readonly DateTime dateTime;

        private Time(DateTime dateTime)
        {
            this.dateTime = dateTime;
        }

        public static Time From(DateTime dateTime)
        {
            return new Time(dateTime);
        }

        public string DifferenceTo(DateTime other)
        {
            var formattedTime = "";
            if (dateTime.Year != other.Year)
            {
                var tempDateTime = new DateTime(dateTime.Ticks);
                var jahre = 0;
                while (tempDateTime < other)
                {
                    var tempTempDateTime = tempDateTime.AddYears(1);
                    if (tempTempDateTime < other)
                    {
                        tempDateTime = tempTempDateTime;
                        jahre++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (jahre > 0)
                {
                    if (jahre > 1)
                    {
                        formattedTime += $"{jahre} Jahre";
                    }
                    else
                    {
                        formattedTime += $"1 Jahr";
                    }
                }

                var months = 0;
                while (tempDateTime < other)
                {
                    var tempTempDateTime = tempDateTime.AddMonths(1);
                    if (tempTempDateTime < other)
                    {
                        tempDateTime = tempTempDateTime;
                        months++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (months > 0)
                {
                    if (!string.IsNullOrEmpty(formattedTime))
                    {
                        formattedTime += " ";
                    }

                    if (months > 1)
                    {
                        formattedTime += $"{months} Monate";
                    }
                    else
                    {
                        formattedTime += $"1 Monat";
                    }
                }

                var days = 0;
                while (tempDateTime < other)
                {
                    var tempTempDateTime = tempDateTime.AddDays(1);
                    if (tempTempDateTime < other)
                    {
                        tempDateTime = tempTempDateTime;
                        days++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (days > 0)
                {
                    if (!string.IsNullOrEmpty(formattedTime))
                    {
                        formattedTime += " ";
                    }

                    if (days > 1)
                    {
                        formattedTime += $"{days} Tage";
                    }
                    else
                    {
                        formattedTime += $"1 Tag";
                    }
                }

                var hours = 0;
                while (tempDateTime < other)
                {
                    var tempTempDateTime = tempDateTime.AddHours(1);
                    if (tempTempDateTime < other)
                    {
                        tempDateTime = tempTempDateTime;
                        hours++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (hours > 0)
                {
                    if (!string.IsNullOrEmpty(formattedTime))
                    {
                        formattedTime += " ";
                    }

                    if (hours > 1)
                    {
                        formattedTime += $"{hours} Stunden";
                    }
                    else
                    {
                        formattedTime += $"1 Stunde";
                    }
                }

                var minutes = 0;
                while (tempDateTime < other)
                {
                    var tempTempDateTime = tempDateTime.AddMinutes(1);
                    if (tempTempDateTime < other)
                    {
                        tempDateTime = tempTempDateTime;
                        minutes++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (minutes > 0)
                {
                    if (!string.IsNullOrEmpty(formattedTime))
                    {
                        formattedTime += " ";
                    }

                    if (minutes > 1)
                    {
                        formattedTime += $"{minutes} Minuten";
                    }
                    else
                    {
                        formattedTime += $"1 Minute";
                    }
                }

                var seconds = 0;
                while (tempDateTime < other)
                {
                    var tempTempDateTime = tempDateTime.AddSeconds(1);
                    if (tempTempDateTime < other)
                    {
                        tempDateTime = tempTempDateTime;
                        seconds++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (seconds > 0)
                {
                    if (!string.IsNullOrEmpty(formattedTime))
                    {
                        formattedTime += " ";
                    }

                    if (seconds > 1)
                    {
                        formattedTime += $"{minutes} Sekunden";
                    }
                    else
                    {
                        formattedTime += $"1 Sekunde";
                    }
                }
            }

            return formattedTime;
        }
    }
}
