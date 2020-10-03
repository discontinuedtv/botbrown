namespace BotBrown.ChatCommands
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using BotBrown.Workers.Twitch;
    using System;
    using System.Threading.Tasks;
    using Serilog;

    public class ViewDeathCommand : BaseChatCommand
    {
        private readonly IEventBus eventBus;
        private readonly ITwitchApiWrapper twitchApiWrapper;
        private readonly IConfigurationManager configurationManager;
        private readonly ILogger logger;

        public ViewDeathCommand(IEventBus eventBus, ITwitchApiWrapper twitchApiWrapper, IConfigurationManager configurationManager, ILogger logger)
        {
            this.eventBus = eventBus;
            this.twitchApiWrapper = twitchApiWrapper;
            this.configurationManager = configurationManager;
            this.logger = logger;
        }

        public override UserType ElligableUserType => UserType.All;

        public override string[] Commands => new[] { "death" };

        public override TimeSpan Cooldown => TimeSpan.FromSeconds(30);

        public override bool ShouldContinue => false;

        public override async Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            try
            {
                var game = await twitchApiWrapper.GetCurrentGame();
                if(string.IsNullOrEmpty(game))
                {
                    logger.Warning("Es konnte kein Spiel ermittelt werden.");
                    return;
                }

                var channelName = chatCommandReceivedEvent.ChannelName;
                var deathCounter = configurationManager.LoadConfiguration<DeathCounterConfiguration>(ConfigurationFileConstants.DeathCounter);
                var currentCounter = deathCounter.GetDeath(game);

                if (currentCounter > 0)
                {
                    eventBus.Publish(new SendChannelMessageRequestedEvent($"Im Spiel {game} sind wir bereits {currentCounter}-mal gestorben.", channelName));
                }
                else
                {
                    eventBus.Publish(new SendChannelMessageRequestedEvent($"Im Spiel {game} sind wir bisher noch nie gestorben.", channelName));
                }
            }
            catch (Exception e)
            {
                logger.Error("Ein Fehler ist in ViewDeathCommand aufgetreten. Exception: {e}", e);
            }

            return;
        }
    }
}
