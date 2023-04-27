namespace BotBrown.Workers.TextToSpeech
{
    using Amazon.Polly;
    using BotBrown.Configuration;
    using NAudio.Wave;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;

    public class AmazonTextToSpeechProcessor : ITextToSpeechProcessor
    {
        private readonly IConfigurationManager configurationManager;
        private readonly ISoundPathProvider soundPathProvider;
        public string Engine => "Amazon";
        private string languages;

        public string TextToSpeechLanguages
        {
            get
            {
                if (string.IsNullOrEmpty(languages))
                {
                    using var pollyClient = GetPollyClient();
                    languages = string.Join(", ", pollyClient.DescribeVoices(new Amazon.Polly.Model.DescribeVoicesRequest()).Voices.Select(voice => voice.LanguageName));
                }

                return languages;
            }
        }

        public AmazonTextToSpeechProcessor(IConfigurationManager configurationManager, ISoundPathProvider soundPathProvider)
        {
            this.configurationManager = configurationManager;
            this.soundPathProvider = soundPathProvider;
        }

        public void Speak(ChannelUser user, Func<ChannelUser, string> messageAction)
        {
            if (!CheckIfTextToSpeechIsActive())
            {
                return;
            }

            using var pollyClient = GetPollyClient();
            var language = GetDesiredLanguage(user);
            if (string.IsNullOrEmpty(language))
            {
                Speak(messageAction(user), null, pollyClient);
            }
            else
            {
                var voice = pollyClient.DescribeVoices(new Amazon.Polly.Model.DescribeVoicesRequest()).Voices.FirstOrDefault(x => x.LanguageName == language);
                Speak(messageAction(user), voice?.Id, pollyClient);
            }
        }

        public void Speak(string message)
        {
            if (!CheckIfTextToSpeechIsActive())
            {
                return;
            }

            Speak(message, null, null);
        }

        private AmazonPollyClient GetPollyClient()
        {
            var awsConfiguration = configurationManager.LoadConfiguration<AmazonAWSConfiguration>();
            return new AmazonPollyClient(awsConfiguration.AWSAccessKeyId, awsConfiguration.AWSSecretKey, Amazon.RegionEndpoint.EUCentral1);
        }

        private void Speak(string message, VoiceId voiceId, AmazonPollyClient pollyClient)
        {
            var audioConfiguration = configurationManager.LoadConfiguration<AudioConfiguration>();
            audioConfiguration.InitializeConfiguration();

            const int volume = 100;

            if (pollyClient == null)
            {
                pollyClient = GetPollyClient();
            }

            if (voiceId == null)
            {
                voiceId = pollyClient.DescribeVoices(new Amazon.Polly.Model.DescribeVoicesRequest { LanguageCode = "de-DE" }).Voices.FirstOrDefault()?.Id;

                if (voiceId == null)
                { return; }
            }

            string ttsPath = Path.Combine(soundPathProvider.Path, "tts");
            if (!Directory.Exists(ttsPath))
            {
                Directory.CreateDirectory(ttsPath);
            }

            var synthesizeResponse = pollyClient.SynthesizeSpeech(new Amazon.Polly.Model.SynthesizeSpeechRequest() { OutputFormat = OutputFormat.Mp3, Text = message, VoiceId = voiceId });
            string ttsFilePath = Path.Combine(ttsPath, "tts.mp3");
            using var fileStream = new FileStream(ttsFilePath, FileMode.Create);
            synthesizeResponse.AudioStream.CopyTo(fileStream);

            synthesizeResponse.AudioStream.Close();
            synthesizeResponse.AudioStream.Dispose();

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

        private string GetDesiredLanguage(ChannelUser user)
        {
            GreetingConfiguration greetingConfiguration = configurationManager.LoadConfiguration<GreetingConfiguration>();
            return greetingConfiguration.RetrieveDesiredLanguage(user.UserId);
        }

        private bool CheckIfTextToSpeechIsActive()
        {
            GeneralConfiguration generalConfiguration = configurationManager.LoadConfiguration<GeneralConfiguration>();
            return generalConfiguration.ActivateTextToSpeech && generalConfiguration.TextToSpeechEngine == "Amazon";
        }

        public bool TryGetLanguage(string requestedLanguage, out string language)
        {
            throw new NotImplementedException();
        }
    }
}
