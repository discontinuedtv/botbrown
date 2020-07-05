namespace BotBrownCore
{
    using System;
    using System.Media;

    public class Command : IDisposable
    {
        private SoundPlayer player;

        public Command(string name, int cooldownInSeconds, string filename, int volume)
        {
            Name = name;
            CooldownInSeconds = cooldownInSeconds;
            Filename = filename;
            player = new SoundPlayer
            {
                SoundLocation = Environment.CurrentDirectory + $"/{filename}"
            };
        }

        public string Name { get; }

        public string Filename { get; }

        public int CooldownInSeconds { get; }

        public DateTimeOffset Cooldown { get; set; }

        public void Dispose()
        {
            player.Dispose();
        }

        public void Execute()
        {
            if (Cooldown > DateTimeOffset.Now)
            {
                return;
            }

            Cooldown = DateTimeOffset.Now.AddSeconds(CooldownInSeconds);
            player.Play();
        }
    }
}