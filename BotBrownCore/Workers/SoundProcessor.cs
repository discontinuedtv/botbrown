namespace BotBrown.Workers
{
    using BotBrown.Configuration;
    using BotBrownCore.Configuration;
    using NAudio.Wave;
    using System;
    using System.IO;
    using System.Threading;

    public class SoundProcessor : ISoundProcessor
    {
        private readonly IConfigurationManager configurationManager;
        private readonly ISoundPathProvider soundPathProvider;

        public SoundProcessor(IConfigurationManager configurationManager, ISoundPathProvider soundPathProvider)
        {
            this.configurationManager = configurationManager;
            this.soundPathProvider = soundPathProvider;
        }

        public void Play(string filename, float volume)
        {
            var configuration = configurationManager.LoadConfiguration<AudioConfiguration>();

            var pathToFile = Path.Combine(soundPathProvider.Path, filename);
            if (!File.Exists(pathToFile))
            {
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