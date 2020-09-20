namespace BotBrown.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public class GreetingConfiguration : IChangeableConfiguration
    {
        public Dictionary<string, string> Greetings { get; set; }
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
    }
}