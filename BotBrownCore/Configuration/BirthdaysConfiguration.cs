namespace BotBrown.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    [ConfigurationFile(ConfigurationFileConstants.Birthdays)]
    public partial class BirthdaysConfiguration : IChangeableConfiguration
    {
        public Dictionary<DayMonth, List<Birthday>> Birthdays { get; set; } = new Dictionary<DayMonth, List<Birthday>>();

        public event PropertyChangedEventHandler PropertyChanged;

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
            foreach(var key in Birthdays.Keys)
            {
                Birthdays[key] = Birthdays[key].Where(x => x.UserId != userId).ToList();
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Birthdays)));
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
