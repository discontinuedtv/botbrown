namespace BotBrown.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    [ConfigurationFile(ConfigurationFileConstants.Facts)]
    public class FactConfiguration : IChangeableConfiguration
    {
        public Dictionary<string, string> Facts { get; set; } = new Dictionary<string, string>();

        public event PropertyChangedEventHandler PropertyChanged;

        public string? GetFact(string key)
        {
            if(Facts.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }

        public string? GetRandomFact()
        {
            var values = Facts.Values;

            if(values.Count == 0)
            {
                return null;
            }

            var index = new Random().Next(0, values.Count - 1);
            return values.ElementAt(index);
        }

        public string AddFact(string key, string fact)
        {
            Facts[key] = fact;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Facts)));
            return fact;
        }

        public void RemoveFact(string key)
        {
            Facts.Remove(key);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Facts)));
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
