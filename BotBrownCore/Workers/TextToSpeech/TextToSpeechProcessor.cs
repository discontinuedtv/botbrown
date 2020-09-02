namespace BotBrown.Workers.TextToSpeech
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Speech.Synthesis;
    using System.Text;
    using BotBrown.Configuration;

    public class TextToSpeechProcessor : ITextToSpeechProcessor
    {
        private readonly IConfigurationManager configurationManager;
        private readonly SpeechSynthesizer synth = new SpeechSynthesizer();
        private readonly IDictionary<string, string> availableLanguages = new Dictionary<string, string>();
        private string languages;

        public TextToSpeechProcessor(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
            synth.SetOutputToDefaultAudioDevice();
            RegisterAvailableLanguages();
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

            string languageForProcessing = GetDesiredLanguage(user);

            synth.SelectVoice(languageForProcessing);
            synth.Speak(messageAction(user));
        }

        public void Speak(string message)
        {
            bool isTextToSpeechActive = CheckIfTextToSpeechIsActive();
            if (!isTextToSpeechActive)
            {
                return;
            }

            synth.Speak(message);
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
            ReadOnlyCollection<InstalledVoice> voices = synth.GetInstalledVoices();

            foreach (InstalledVoice voice in voices)
            {
                string[] languageName = voice.VoiceInfo.Culture.DisplayName.ToLower().Split(' ');
                availableLanguages.Add(languageName[0], voice.VoiceInfo.Name);
            }
        }
    }
}