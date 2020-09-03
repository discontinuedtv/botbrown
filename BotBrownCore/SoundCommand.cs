namespace BotBrown
{
    using BotBrownCore.Configuration;
    using NAudio.Wave;
    using System;
    using System.Threading;

    public class SoundCommand : ICommand, IDisposable
    {
        private readonly AudioConfiguration configuration;

        public SoundCommand(string shortcut, string name, int cooldownInSeconds, string filename, float volume, AudioConfiguration audioConfiguration)
        {
            Shortcut = shortcut;
            Name = name;
            CooldownInSeconds = cooldownInSeconds;
            Filename = filename;
            Volume = volume;
            configuration = audioConfiguration;
        }

        public string Name { get; }

        public string Filename { get; }

        public int CooldownInSeconds { get; }

        public DateTimeOffset Cooldown { get; set; }

        public string Shortcut { get; }

        public float Volume { get; }

        public void Dispose()
        {
        }

        public void Execute()
        {
            if (Cooldown > DateTimeOffset.Now)
            {
                return;
            }

            Cooldown = DateTimeOffset.Now.AddSeconds(CooldownInSeconds);

            using (var reader = new MediaFoundationReader(Filename))
            using (var outputStream = new WasapiOut(configuration.SelectedSoundCommandDevice, NAudio.CoreAudioApi.AudioClientShareMode.Shared, false, 10))
            {
                outputStream.Init(reader);
                outputStream.Volume = Volume;
                outputStream.Play();

                while(outputStream.PlaybackState != PlaybackState.Stopped)
                {
                    Thread.Sleep(5);
                }
            }
        }
    }
}