namespace BotBrown.Configuration
{
    using BotBrown;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel;

    [ConfigurationFile(ConfigurationFileConstants.Greetings)]
    public class GreetingConfiguration : IChangeableConfiguration
    {
        public Dictionary<string, string> Greetings { get; set; } = new Dictionary<string, string>();

        [JsonIgnore]
        public string Filename => ConfigurationFileConstants.Greetings;

        public event PropertyChangedEventHandler PropertyChanged;

        internal void AddGreeting(ChannelUser user, string language)
        {
            Greetings[user.UserId] = language;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Greetings)));
        }

        internal bool TryGetValue(string userId, out string language)
        {
            return Greetings.TryGetValue(userId, out language);
        }

        internal string RetrieveDesiredLanguage(string userId)
        {
            if (!Greetings.TryGetValue(userId, out string desiredLanguage))
            {
                return "Microsoft Hedda Desktop";
            }

            return desiredLanguage;
        }

        public bool IsValid()
        {
            return true;
        }
    }
}