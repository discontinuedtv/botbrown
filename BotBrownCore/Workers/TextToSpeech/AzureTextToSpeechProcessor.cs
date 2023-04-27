namespace BotBrown.Workers.TextToSpeech
{
    using BotBrown.Configuration;
    using Microsoft.CognitiveServices.Speech;
    using NAudio.Wave;
    using System;
    using System.IO;
    using System.Threading;
    using Serilog;

    public class AzureTextToSpeechProcessor : ITextToSpeechProcessor
    {
        private readonly ISoundPathProvider soundPathProvider;
        private readonly IConfigurationManager configurationManager;
        private readonly ILogger logger;

        public string TextToSpeechLanguages => "deutsch";

        public string Engine => "Azure";

        public AzureTextToSpeechProcessor(ISoundPathProvider soundPathProvider, IConfigurationManager configurationManager, ILogger logger)
        {
            this.soundPathProvider = soundPathProvider;
            this.configurationManager = configurationManager;
            this.logger = logger.ForContext<AzureTextToSpeechProcessor>();
        }

        public void Speak(ChannelUser user, Func<ChannelUser, string> messageAction)
        {
            Speak(messageAction(user));
        }

        public void Speak(string message)
        {
            var audioConfiguration = configurationManager.LoadConfiguration<AudioConfiguration>();
            var azureConfiguration = configurationManager.LoadConfiguration<AzureConfiguration>();

            if(!azureConfiguration.IsValid())
            {
                logger.Error("Die AzureConfiguration ist nicht valide.");
                return;
            }

            audioConfiguration.InitializeConfiguration();
            const int volume = 75;

            var config = SpeechConfig.FromSubscription(azureConfiguration.SubscriptionKey, "westeurope");

            config.OutputFormat = OutputFormat.Detailed;
            config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio24Khz160KBitRateMonoMp3);
            config.SpeechSynthesisLanguage = "de-de";

            using var synthesizer = new SpeechSynthesizer(config);
            var synthesizeTask = synthesizer.SpeakTextAsync(message);
            var result = synthesizeTask.GetAwaiter().GetResult();

            string ttsPath = Path.Combine(soundPathProvider.Path, "tts");
            if (!Directory.Exists(ttsPath))
            {
                Directory.CreateDirectory(ttsPath);
            }

            string ttsFilePath = Path.Combine(ttsPath, "tts.mp3");
            File.WriteAllBytes(ttsFilePath, result.AudioData);

            using var reader = new MediaFoundationReader(ttsFilePath);
            using var volumeStream = new WaveChannel32(reader);
            using var outputStream = new WasapiOut(audioConfiguration.SelectedSoundCommandDevice, NAudio.CoreAudioApi.AudioClientShareMode.Shared, false, 10);
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

        public bool TryGetLanguage(string requestedLanguage, out string language)
        {
            language = "deutsch";
            return true;
        }
    }
}
