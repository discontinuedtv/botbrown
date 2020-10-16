namespace BotBrown.Configuration
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    [ConfigurationFile(ConfigurationFileConstants.Birthdays)]
    public partial class BirthdaysConfiguration : IChangeableConfiguration
    {
        [JsonConverter(typeof(CustomDictionaryConverter<DayMonth, List<Birthday>>))]
        public Dictionary<DayMonth, List<Birthday>> Birthdays { get; set; } = new Dictionary<DayMonth, List<Birthday>>();

        public event PropertyChangedEventHandler PropertyChanged;

        public bool ContainsBirthdayForDate(DateTime dateToCheck)
        {
            var dayMonth = new DayMonth { Day = dateToCheck.Day, Month = dateToCheck.Month };
            return Birthdays.ContainsKey(dayMonth);
        }

        public void AddBirthday(DateTime birthday, string userId)
        {
            Birthday item = new Birthday { Day = birthday, Gratulated = new List<int>(), UserId = userId };
            DayMonth? dayMonth = new DayMonth { Day = birthday.Day, Month = birthday.Month };
            if (Birthdays.ContainsKey(dayMonth))
            {
                Birthdays[dayMonth].Add(item);
            }
            else
            {
                Birthdays[dayMonth] = new List<Birthday> { item };
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Birthdays)));
        }

        public void DeleteBirthday(string userId)
        {
            foreach(var key in Birthdays.Keys.ToList())
            {
                Birthdays[key] = Birthdays[key].Where(x => x.UserId != userId).ToList();
                if (!Birthdays[key].Any())
                {
                    Birthdays.Remove(key);
                }
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Birthdays)));
        }

        public bool IsValid()
        {
            return true;
        }

        public List<Birthday> GetBirthdays(DateTime date)
        {
            var dayMonth = new DayMonth { Day = date.Day, Month = date.Month };
            return Birthdays[dayMonth];
        }

        public void MarkChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BirthdaysConfiguration)));
        }
    }
}
