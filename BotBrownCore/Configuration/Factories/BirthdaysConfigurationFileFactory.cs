namespace BotBrown.Configuration.Factories
{
    using System;
    using System.Collections.Generic;

    public class BirthdaysConfigurationFileFactory : IConfigurationFileFactory<BirthdaysConfiguration>
    {
        public BirthdaysConfiguration CreateDefaultConfiguration()
        {
            return new BirthdaysConfiguration
            {
                Birthdays = new Dictionary<DayMonth, List<Birthday>>
                {
                    { new DayMonth{ Day = 17, Month = 11}, new List<Birthday> { new Birthday { Day = DateTime.Parse("17.11.1986"), Gratulated = new List<int>(), UserId = 476539607 } } }
                }
            };
        }
    }
}
