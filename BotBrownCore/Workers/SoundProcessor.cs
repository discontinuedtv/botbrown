namespace BotBrown.Workers
{
    using System;
    using System.IO;
    using System.Threading;
    using BotBrown.Configuration;
    using NAudio.Wave;
    using Serilog;

    public class SoundProcessor : ISoundProcessor
    {
        private readonly IConfigurationManager configurationManager;
        private readonly ISoundPathProvider soundPathProvider;
        private readonly ILogger logger;

        public SoundProcessor(IConfigurationManager configurationManager, ISoundPathProvider soundPathProvider, ILogger logger)
        {
            this.configurationManager = configurationManager;
            this.soundPathProvider = soundPathProvider;
            this.logger = logger.ForContext<SoundProcessor>();
        }

        public void Play(string filename, float volume)
        {
            var configuration = configurationManager.LoadConfiguration<AudioConfiguration>();
            configuration.InitializeConfiguration();

            var pathToFile = Path.Combine(soundPathProvider.Path, filename);
            if (!File.Exists(pathToFile))
            {
                return;
            }

            if (volume == 0f)
            {
                logger.Warning($"Die Lautstärke für '{filename}' liegt bei 0 und kann nicht gehört werden.");
                return;
            }

            using var reader = new MediaFoundationReader(pathToFile);
            using var volumeStream = new WaveChannel32(reader);
            using var outputStream = new WasapiOut(configuration.SelectedSoundCommandDevice, NAudio.CoreAudioApi.AudioClientShareMode.Shared, false, 10);
            volumeStream.Volume = NormalizeVolume(volume);
            outputStream.Init(volumeStream);
            outputStream.Play();

            Thread.Sleep(reader.TotalTime.Add(TimeSpan.FromMilliseconds(100)));

            outputStream.Stop();
        }

        private float NormalizeVolume(float rawVolume)
        {
            float volume = Math.Min(rawVolume, 100);
            return volume / 100;
        }
    }
}