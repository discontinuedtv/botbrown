namespace BotBrown.Workers.TextToSpeech
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Speech.AudioFormat;
    using System.Speech.Synthesis;
    using System.Text;
    using System.Threading;
    using BotBrown.Configuration;
    using BotBrownCore.Configuration;
    using NAudio.CoreAudioApi;
    using NAudio.Wave;

    public class TextToSpeechProcessor : ITextToSpeechProcessor
    {
        private const int BitResolution = 16;

        private readonly IConfigurationManager configurationManager;
        private readonly IDictionary<string, string> availableLanguages = new Dictionary<string, string>();
        private readonly AudioConfiguration audioConfiguration;
        private bool isInitialized;
        private string languages;

        public TextToSpeechProcessor(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
            RegisterAvailableLanguages();

            audioConfiguration = configurationManager.LoadConfiguration<AudioConfiguration>(ConfigurationFileConstants.Audio);
        }

        public string TextToSpeechLanguages
        {
            get
            {
                if (string.IsNullOrEmpty(languages))
                {
                    InitializeLanguages();
                }

                return languages;
            }
        }

        public void Speak(ChannelUser user, Func<ChannelUser, string> messageAction)
        {
            bool isTextToSpeechActive = CheckIfTextToSpeechIsActive();
            if (!isTextToSpeechActive)
            {
                return;
            }

            InitializeAudioDevices();

            using (var stream = new MemoryStream())
            using (var output = new WasapiOut(audioConfiguration.SelectedTTSDevice, AudioClientShareMode.Shared, false, 100))
            using (var provider = new RawSourceWaveStream(stream, new WaveFormat(44100, BitResolution, 2)))
            using (var synth = new SpeechSynthesizer())
            {
                synth.SetOutputToAudioStream(stream, new SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Stereo));
                synth.SelectVoice(GetDesiredLanguage(user));
                synth.Speak(messageAction(user));

                stream.Seek(0, SeekOrigin.Begin);

                //output.Volume = 0.25f;
                output.Init(provider);
                output.Play();

                while (output.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(10);
                }
            }
        }

        public void Speak(string message)
        {
            bool isTextToSpeechActive = CheckIfTextToSpeechIsActive();
            if (!isTextToSpeechActive)
            {
                return;
            }

            InitializeAudioDevices();

            using (var stream = new MemoryStream())
            using (var output = new WasapiOut(audioConfiguration.SelectedTTSDevice, AudioClientShareMode.Shared, true, 100))
            using (var provider = new RawSourceWaveStream(stream, new WaveFormat(44100, BitResolution, 2)))
            using (var synth = new SpeechSynthesizer())
            {
                synth.SetOutputToAudioStream(stream, new SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Stereo));
                synth.Speak(message);

                stream.Seek(0, SeekOrigin.Begin);

                output.Volume = 0.25f;
                output.Init(provider);
                output.Play();

                while (output.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(10);
                }
            }
        }

        public bool TryGetLanguage(string requestedLanguage, out string language)
        {
            return availableLanguages.TryGetValue(requestedLanguage, out language);
        }

        private void InitializeAudioDevices()
        {
            if (isInitialized)
            {
                return;
            }

            audioConfiguration.InitializeConfiguration();
        }

        private string GetDesiredLanguage(ChannelUser user)
        {
            GreetingConfiguration greetingConfiguration = configurationManager.LoadConfiguration<GreetingConfiguration>(ConfigurationFileConstants.Greetings);
            return greetingConfiguration.RetrieveDesiredLanguage(user.UserId);
        }

        private bool CheckIfTextToSpeechIsActive()
        {
            GeneralConfiguration generalConfiguration = configurationManager.LoadConfiguration<GeneralConfiguration>(ConfigurationFileConstants.General);
            return generalConfiguration.ActivateTextToSpeech;
        }

        private void InitializeLanguages()
        {
            var sb = new StringBuilder();
            sb.Append("Dies sind die verfügbaren Sprachen: ");
            sb.Append(string.Join(", ", availableLanguages.Keys));
            languages = sb.ToString();
        }

        private void RegisterAvailableLanguages()
        {
            using (var synth = new SpeechSynthesizer())
            {
                ReadOnlyCollection<InstalledVoice> voices = synth.GetInstalledVoices();

                foreach (InstalledVoice voice in voices)
                {
                    string[] languageName = voice.VoiceInfo.Culture.DisplayName.ToLower().Split(' ');
                    availableLanguages.Add(languageName[0], voice.VoiceInfo.Name);
                }
            }
        }
    }
}