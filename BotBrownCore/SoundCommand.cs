namespace BotBrown
{
    using System;
    using System.IO;

    public class SoundCommand : ICommand
    {
        public SoundCommand(string shortcut, string name, int cooldownInSeconds, string filename, float volume)
        {
            Shortcut = shortcut;
            Name = name;
            CooldownInSeconds = cooldownInSeconds;
            Filename = filename;
            Volume = volume;
        }

        public string Name { get; }

        public string Filename { get; }

        public int CooldownInSeconds { get; }

        public DateTimeOffset Cooldown { get; set; }

        public string Shortcut { get; }

        public float Volume { get; }

        public bool ShouldExecute
        {
            get
            {
                if (Cooldown > DateTimeOffset.Now)
                {
                    return false;
                }

                return true;
            }
        }

        public void MarkAsExecuted()
        {
            Cooldown = DateTimeOffset.Now.AddSeconds(CooldownInSeconds);
        }
    }
}