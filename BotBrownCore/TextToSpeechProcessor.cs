namespace BotBrownCore
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Speech.Synthesis;
    using System.Text;
    using BotBrownCore.Configuration;

    internal class TextToSpeechProcessor : ITextToSpeechProcessor
    {
        private readonly SpeechSynthesizer synth = new SpeechSynthesizer();
        private readonly IDictionary<string, string> availableLanguages = new Dictionary<string, string>();
        private string languages;
        private bool isTextToSpeechActive;

        public TextToSpeechProcessor()
        {
            synth.SetOutputToDefaultAudioDevice();
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

        public void RegisterAvailableLanguages()
        {
            ReadOnlyCollection<InstalledVoice> voices = synth.GetInstalledVoices();

            foreach (InstalledVoice voice in voices)
            {
                string[] languageName = voice.VoiceInfo.Culture.DisplayName.ToLower().Split(' ');
                availableLanguages.Add(languageName[0], voice.VoiceInfo.Name);
            }
        }

        public void Speak(string username, string language, Func<string, string> messageAction)
        {
            if (!isTextToSpeechActive)
            {
                return;
            }

            synth.SelectVoice(language);
            synth.Speak(messageAction(username));
        }

        private void InitializeLanguages()
        {
            var sb = new StringBuilder();
            sb.Append("Dies sind die verfügbaren Sprachen: ");
            sb.Append(string.Join(", ", availableLanguages.Keys));
            languages = sb.ToString();
        }

        internal bool TryGetLanguage(string requestedLanguage, out string language)
        {
            return availableLanguages.TryGetValue(requestedLanguage, out language);
        }

        internal void Configure(GeneralConfiguration generalConfiguration)
        {
            isTextToSpeechActive = generalConfiguration.ActivateTextToSpeech;
        }
    }
}