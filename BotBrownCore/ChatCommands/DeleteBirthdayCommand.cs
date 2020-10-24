namespace BotBrown.ChatCommands
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using BotBrown.Workers.Twitch;
    using System;
    using System.Threading.Tasks;

    public class DeleteBirthdayCommand : BaseChatCommand
    {
        private readonly ITwitchApiWrapper twitchApiWrapper;
        private readonly IConfigurationManager configurationManager;
        private readonly IEventBus eventBus;

        public DeleteBirthdayCommand(ITwitchApiWrapper twitchApiWrapper, IConfigurationManager configurationManager, IEventBus eventBus)
        {
            this.twitchApiWrapper = twitchApiWrapper;
            this.configurationManager = configurationManager;
            this.eventBus = eventBus;
        }

        public override UserType ElligableUserType => UserType.Editor;

        public override string[] Commands => new[] { "deletebirthday" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override async Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            var userId = await twitchApiWrapper.GetUserIdByUsername(chatCommandReceivedEvent.CommandArgs);
            var birthdayConfiguration = configurationManager.LoadConfiguration<BirthdaysConfiguration>();
            birthdayConfiguration.DeleteBirthday(userId);

            eventBus.Publish(new SendChannelMessageRequestedEvent("Geburstag erfolgreich gelöscht!", chatCommandReceivedEvent.ChannelName));
        }
    }
}
