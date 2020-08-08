﻿namespace BotBrownCore
{
    using System;
    using System.Media;

    public class SoundCommand : ICommand, IDisposable
    {
        private SoundPlayer player;

        public SoundCommand(string shortcut, string name, int cooldownInSeconds, string filename, int volume)
        {
            Shortcut = shortcut;
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

        public string Shortcut { get; }

        public void Dispose()
        {
            player.Dispose();
        }

        public void Execute(IBotExecutionContext context)
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