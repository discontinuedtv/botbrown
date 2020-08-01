namespace BotBrownCore.Configuration
{
    using System.Collections.Generic;

    internal class GreetingConfiguration : IConfiguration
    {
        public Dictionary<string, string> Greetings { get; set; } = new Dictionary<string, string>();

        internal void AddGreeting(string userId, string language)
        {
            Greetings[userId] = language;
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