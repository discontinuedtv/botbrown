namespace BotBrown.ChatCommands
{
    using System;
    using System.Threading.Tasks;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using BotBrown.Workers.Twitch;

    public class FollowageCommand : BaseChatCommand
    {
        private readonly ITwitchApiWrapper twitchApi;
        private readonly IEventBus eventBus;

        public FollowageCommand(ITwitchApiWrapper twitchApi, IEventBus eventBus)
        {
            this.twitchApi = twitchApi;
            this.eventBus = eventBus;
        }

        public override UserType ElligableUserType => UserType.All;

        public override string[] Commands => new[] { "followage" };

        public override TimeSpan Cooldown => TimeSpan.FromSeconds(10);

        public override bool ShouldContinue => true;

        public override async Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            if (!string.IsNullOrEmpty(chatCommandReceivedEvent.CommandArgs))
            {
                return;
            }

            var followage = await twitchApi.GetFollowSince(chatCommandReceivedEvent.User.UserId);

            if (followage.HasValue)
            {
                var time = Time.From(followage.Value);
                var difference = time.DifferenceTo(DateTime.Now);

                eventBus.Publish(new SendChannelMessageRequestedEvent(
                    $"{chatCommandReceivedEvent.User.Username} folgt uns seit: {difference}",
                    chatCommandReceivedEvent.ChannelName));
            }
        }
    }
}
