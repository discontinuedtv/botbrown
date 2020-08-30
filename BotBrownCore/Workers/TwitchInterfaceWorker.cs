using BotBrownCore.Configuration;
using BotBrownCore.Events;
using BotBrownCore.Workers.Twitch;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BotBrownCore.Workers
{
    public sealed class TwitchInterfaceWorker : IDisposable
    {
        private readonly IEventBus bus;
        private readonly ITwitchClientWrapper clientWrapper;
        private readonly ITwitchApiWrapper apiWrapper;
        private readonly ILogger logger;
        private readonly IConfigurationManager configurationManager;
        private Guid identifier = Guid.NewGuid();
        private TwitchConfiguration twitchConfiguration;

        public TwitchInterfaceWorker(IEventBus bus, ITwitchClientWrapper clientWrapper, ITwitchApiWrapper apiWrapper, ILogger logger, IConfigurationManager configurationManager)
        {
            this.bus = bus;
            this.clientWrapper = clientWrapper;
            this.apiWrapper = apiWrapper;
            this.logger = logger;
            this.configurationManager = configurationManager;
        }

        public async Task<bool> Execute(CancellationToken cancellationToken)
        {
            try
            {
                twitchConfiguration = configurationManager.LoadConfiguration<TwitchConfiguration>(ConfigurationFileConstants.Twitch);
                logger.Log("Twitch Konfiguration wurden geladen.");

                if (!twitchConfiguration.IsValid())
                {
                    logger.Log("Die Twitch Konfiguration ist nich valide");
                    return false;
                }

                clientWrapper.ConnectToTwitch(twitchConfiguration);
                apiWrapper.ConnectToTwitch(twitchConfiguration);
            }
            catch (Exception e)
            {
                logger.Log("Der Bot wurde aufgrund eines Fehlers beendet");
                logger.Error(e);
            }

            bus.SubscribeToTopic<SendChannelMessageRequestedEvent>(identifier);
            bus.SubscribeToTopic<SendWhisperMessageRequestedEvent>(identifier);
          
            while (!cancellationToken.IsCancellationRequested)
            {
                if (bus.TryConsume(identifier, out SendChannelMessageRequestedEvent message))
                {
                    SendChannelMessage(message);
                }

                if (bus.TryConsume(identifier, out SendWhisperMessageRequestedEvent whisper))
                {
                    SendWhisperMessage(whisper);
                }

                await Task.Delay(100);
            }

            return true;
        }

        public void Dispose()
        {
            clientWrapper.Stop();
            apiWrapper.Stop();
        }

        private void SendWhisperMessage(SendWhisperMessageRequestedEvent whisperRequest)
        {
            clientWrapper.SendWhisper(whisperRequest.User.RealUsername, whisperRequest.Message);
        }

        private void SendChannelMessage(SendChannelMessageRequestedEvent chatMessageRequest)
        {
            clientWrapper.SendMessage(chatMessageRequest.ChannelName, chatMessageRequest.Message);
        }
    }
}
