namespace BotBrown
{
    public class CommandDefinition
    {
        private SoundCommand soundCommand;

        public string Shortcut { get; set; }

        public string Name { get; set; }

        public string Filename { get; set; }

        public int CooldownInSeconds { get; set; }

        public float Volume { get; set; }

        internal SoundCommand CreateCommand()
        {
            return soundCommand ??= new SoundCommand(Shortcut, Name, CooldownInSeconds, Filename, Volume);
        }
    }
}