namespace BotBrown
{
    public class CommandDefinition
    {
        private SoundCommand soundCommand = null;

        public string Shortcut { get; set; }

        public string Name { get; set; }

        public string Filename { get; set; }

        public int CooldownInSeconds { get; set; }

        public float Volume { get; set; }

        internal SoundCommand CreateCommand()
        {
            soundCommand ??= new SoundCommand(Shortcut, Name, CooldownInSeconds, Filename, Volume);
            return soundCommand;
        }
    }
}