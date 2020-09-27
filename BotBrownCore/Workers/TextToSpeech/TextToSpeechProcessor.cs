namespace BotBrown.Workers.TextToSpeech
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Speech.Synthesis;
    using System.Text;
    using System.Threading;
    using BotBrown.Configuration;
    using BotBrownCore.Configuration;
    using NAudio.CoreAudioApi;
    using NAudio.Wave;
    using SpeechLib;

    public class TextToSpeechProcessor : ITextToSpeechProcessor
    {
        private const int BitResolution = 16;

        private readonly IConfigurationManager configurationManager;
        private readonly IDictionary<string, string> availableLanguages = new Dictionary<string, string>();
        private readonly AudioConfiguration audioConfiguration;
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

            audioConfiguration.InitializeConfiguration();
            SpeakToOutputDevice(messageAction(user), GetDesiredLanguage(user));
        }

        private void SpeakToOutputDevice(string message, string language)
        {
            const SpeechVoiceSpeakFlags speechFlags = SpeechVoiceSpeakFlags.SVSFlagsAsync;
            SpVoice synth = new SpVoice();
            SpMemoryStream wave = new SpMemoryStream();
            ISpeechObjectTokens voices = synth.GetVoices();
            try
            {
                synth.Rate = 0;
                synth.Volume = 100;

                wave.Format.Type = SpeechAudioFormatType.SAFT44kHz16BitStereo;
                synth.AudioOutputStream = wave;

                if (language != null)
                {
                    foreach (SpObjectToken voice in voices)
                    {
                        if (voice.GetAttribute("Name") == language)
                        {
                            synth.Voice = voice;
                        }
                    }
                }

                synth.Speak(message, speechFlags);
                synth.WaitUntilDone(Timeout.Infinite);

                OutputWaveStream(wave);
            }
            finally
            {
                Marshal.ReleaseComObject(voices);
                Marshal.ReleaseComObject(wave);
                Marshal.ReleaseComObject(synth);
            }
        }

        private void OutputWaveStream(SpMemoryStream waveStream)
        {
            using (var sourceStream = new MemoryStream((byte[])waveStream.GetData()))
            using (var output = new WasapiOut(audioConfiguration.SelectedTTSDevice, AudioClientShareMode.Shared, true, 100))
            using (var provider = new RawSourceWaveStream(sourceStream, new WaveFormat(44100, BitResolution, 2)))
            {
                sourceStream.Seek(0, SeekOrigin.Begin);

                output.Volume = 1f;
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

            audioConfiguration.InitializeConfiguration();
            SpeakToOutputDevice(message, null);
        }

        public bool TryGetLanguage(string requestedLanguage, out string language)
        {
            return availableLanguages.TryGetValue(requestedLanguage, out language);
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