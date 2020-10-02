namespace BotBrown.Workers
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Workers.TextToSpeech;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Serilog;

    public class SoundWorker
    {
        private readonly ISoundProcessor soundProcessor;
        private readonly ITextToSpeechProcessor textToSpeechProcessor;
        private readonly ILogger logger;

        private Guid identifier = Guid.NewGuid();
        private IEventBus bus;

        public SoundWorker(IEventBus bus, ITextToSpeechProcessor textToSpeechProcessor, ISoundProcessor soundProcessor, ILogger logger)
        {
            this.bus = bus;
            this.textToSpeechProcessor = textToSpeechProcessor;
            this.soundProcessor = soundProcessor;
            this.logger = logger.ForContext<SoundWorker>();
        }

        public async Task<bool> Execute(CancellationToken cancellationToken)
        {
            bus.SubscribeToTopic<TextToSpeechEvent>(identifier);
            bus.SubscribeToTopic<PlaySoundEvent>(identifier);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (bus.TryConsume(identifier, out TextToSpeechEvent textToSpeechEvent))
                    {
                        ProcessTextToSpeechEvent(textToSpeechEvent);
                    }

                    if (bus.TryConsume(identifier, out PlaySoundEvent playSoundEvent))
                    {
                        ProcessPlaySoundEvent(playSoundEvent);
                    }

                    await Task.Delay(100, cancellationToken);
                }
                catch (Exception e)
                {
                    logger.Error("Beim Vearbeiten der Soundanfrage ist ein Fehler aufgetreten: {e}", e);
                }
            }

            return true;
        }

        private void ProcessPlaySoundEvent(PlaySoundEvent playSoundEvent)
        {
            soundProcessor.Play(playSoundEvent.Filename, playSoundEvent.Volume);
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
