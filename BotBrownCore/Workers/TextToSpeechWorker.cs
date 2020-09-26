namespace BotBrown.Workers
{
    using BotBrown;
    using BotBrown.Events;
    using BotBrown.Workers.TextToSpeech;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class TextToSpeechWorker
    {
        private readonly ITextToSpeechProcessor textToSpeechProcessor;

        private Guid identifier = Guid.NewGuid();
        private IEventBus bus;

        public TextToSpeechWorker(IEventBus bus, ITextToSpeechProcessor textToSpeechProcessor)
        {
            this.bus = bus;
            this.textToSpeechProcessor = textToSpeechProcessor;
        }

        public async Task<bool> Execute(CancellationToken cancellationToken)
        {
            bus.SubscribeToTopic<TextToSpeechEvent>(identifier);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (bus.TryConsume(identifier, out TextToSpeechEvent message))
                {
                    ProcessTextToSpeechEvent(message);
                }

                await Task.Delay(100);
            }

            return true;
        }

        private void ProcessTextToSpeechEvent(TextToSpeechEvent message)
        {
            if (message is SpeakEvent)
            {
                ChannelUser user = message.User;

                Speak(message.User, u => $"{u.Username} sagt {message.Message}");
                return;
            }

            Speak(message.Message);
        }

        private void Speak(ChannelUser user, Func<ChannelUser, string> messageAction)
        {
            textToSpeechProcessor.Speak(user, messageAction);
        }

        private void Speak(string messageAction)
        {
            textToSpeechProcessor.Speak(messageAction);
        }
    }
}
