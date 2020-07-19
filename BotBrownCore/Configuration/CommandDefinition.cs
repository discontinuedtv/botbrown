namespace BotBrownCore.Configuration
{
    internal class CommandDefinition
    {
        public string Shortcut { get; set; }

        public string Name { get; set; }

        public string Filename { get; set; }

        public int CooldownInSeconds { get; set; }

        internal Command CreateCommand()
        {
            return new Command(Shortcut, Name, CooldownInSeconds, Filename, 100);
        }
    }
}