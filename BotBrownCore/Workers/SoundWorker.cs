namespace BotBrown.Workers
{
    using BotBrown;
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Workers.TextToSpeech;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class SoundWorker
    {
        private readonly ISoundProcessor soundProcessor;
        private readonly IConfigurationManager configurationManager;
        private readonly IList<ITextToSpeechProcessor> textToSpeechProcessors;
        private readonly ILogger logger;
        private readonly IEventBus bus;
        private readonly Guid identifier = Guid.NewGuid();

        public SoundWorker(IEventBus bus, IList<ITextToSpeechProcessor> textToSpeechProcessors, ISoundProcessor soundProcessor, ILogger logger, IConfigurationManager configurationManager)
        {
            this.bus = bus;
            this.textToSpeechProcessors = textToSpeechProcessors;
            this.soundProcessor = soundProcessor;
            this.configurationManager = configurationManager;
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
                catch (TaskCanceledException)
                { }
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
            var generalConfiguration = configurationManager.LoadConfiguration<GeneralConfiguration>();
            if (!generalConfiguration.ActivateTextToSpeech)
            {
                return;
            }

            var textToSpeechProcessor = textToSpeechProcessors.FirstOrDefault(x => x.Engine == generalConfiguration.TextToSpeechEngine);
            textToSpeechProcessor?.Speak(user, messageAction);
        }

        private void Speak(string messageAction)
        {
            var generalConfiguration = configurationManager.LoadConfiguration<GeneralConfiguration>();
            if (!generalConfiguration.ActivateTextToSpeech)
            {
                return;
            }

            var textToSpeechProcessor = textToSpeechProcessors.FirstOrDefault(x => x.Engine == generalConfiguration.TextToSpeechEngine);
            textToSpeechProcessor?.Speak(messageAction);
        }
    }
}
