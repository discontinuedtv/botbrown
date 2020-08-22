namespace BotBrownCore.Configuration
{
    public class CommandDefinition
    {
        public string Shortcut { get; set; }

        public string Name { get; set; }

        public string Filename { get; set; }

        public int CooldownInSeconds { get; set; }

        internal SoundCommand CreateCommand()
        {
            return new SoundCommand(Shortcut, Name, CooldownInSeconds, Filename, 100);
        }
    }
}