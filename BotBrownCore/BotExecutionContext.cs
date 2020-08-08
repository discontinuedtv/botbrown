namespace BotBrownCore
{
    internal class BotExecutionContext : IBotExecutionContext
    {
        public BotExecutionContext(TextToSpeechProcessor ttsProcessor)
        {
            TtsProcessor = ttsProcessor;
        }

        public TextToSpeechProcessor TtsProcessor { get; }
    }
}