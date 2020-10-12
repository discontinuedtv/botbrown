namespace BotBrown.Workers
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Workers.Twitch;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Serilog;

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
            this.logger = logger.ForContext<TwitchInterfaceWorker>();
            this.configurationManager = configurationManager;
        }

        public async Task<bool> Execute(CancellationToken cancellationToken)
        {
            try
            {
                twitchConfiguration = configurationManager.LoadConfiguration<TwitchConfiguration>();
                logger.Information("Twitch Konfiguration wurde geladen.");

                if (!twitchConfiguration.IsValid())
                {
                    logger.Information("Die Twitch Konfiguration ist nicht valide");
                    return false;
                }

                clientWrapper.ConnectToTwitch(twitchConfiguration);
                apiWrapper.ConnectToTwitch(twitchConfiguration);
            }
            catch (Exception e)
            {
                logger.Error("Der Bot wurde aufgrund eines Fehlers beendet. Fehler: {e}", e);
            }

            bus.SubscribeToTopic<SendChannelMessageRequestedEvent>(identifier);
            bus.SubscribeToTopic<SendWhisperMessageRequestedEvent>(identifier);
            bus.SubscribeToTopic<UpdateChannelEvent>(identifier);
          
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

                if(bus.TryConsume(identifier, out UpdateChannelEvent channelUpdate))
                {
                    apiWrapper.UpdateChannel(channelUpdate);
                }

                await Task.Delay(100);
            }

            return true;
        }

        public void Dispose()
        {
            clientWrapper?.Stop();
            apiWrapper?.Stop();
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
