namespace BotBrownCore.Workers.TextToSpeech
{
    using BotBrownCore.Configuration;
    using System;

    public interface ITextToSpeechProcessor
    {
        string TextToSpeechLanguages { get; }

        void Speak(ChannelUser user, Func<ChannelUser, string> messageAction);

        void Speak(string message);

        bool TryGetLanguage(string requestedLanguage, out string language);
    }
}