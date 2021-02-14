namespace BotBrown.ChatCommands
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using BotBrown.Workers.Twitch;
    using System;
    using System.Threading.Tasks;

    public class AddBirthdayCommand : BaseChatCommand
    {
        private readonly ITwitchApiWrapper twitchApiWrapper;
        private readonly IConfigurationManager configurationManager;
        private readonly IEventBus eventBus;

        public AddBirthdayCommand(ITwitchApiWrapper twitchApiWrapper, IConfigurationManager configurationManager, IEventBus eventBus)
        {
            this.twitchApiWrapper = twitchApiWrapper;
            this.configurationManager = configurationManager;
            this.eventBus = eventBus;
        }

        public override UserType ElligableUserType => UserType.Editor;

        public override string[] Commands => new[]{ "addbirthday" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override async Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            var args = chatCommandReceivedEvent.CommandArgs.Split(' ');
            if(args.Length != 2)
            {
                return;
            }

            var userId = await twitchApiWrapper.GetUserIdByUsername(args[0]);
            var birthday = DateTime.Parse(args[1]);

            var birthdayConfig = configurationManager.LoadConfiguration<BirthdaysConfiguration>();
            birthdayConfig.AddBirthday(birthday, userId);

            eventBus.Publish(new SendChannelMessageRequestedEvent("Geburstag erfolgreich angelegt!", chatCommandReceivedEvent.ChannelName));
        }
    }
}
