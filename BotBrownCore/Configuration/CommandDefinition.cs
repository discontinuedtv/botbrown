using BotBrownCore.Configuration;

namespace BotBrown.Configuration
{
    public class CommandDefinition
    {
        public string Shortcut { get; set; }

        public string Name { get; set; }

        public string Filename { get; set; }

        public int CooldownInSeconds { get; set; }

        public float Volume { get; set; }

        internal SoundCommand CreateCommand(AudioConfiguration configuration)
        {
            return new SoundCommand(Shortcut, Name, CooldownInSeconds, Filename, Volume, configuration);
        }
    }
}