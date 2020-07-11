using System;
using System.Collections.Generic;

namespace BotBrownCore
{
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
    }
}