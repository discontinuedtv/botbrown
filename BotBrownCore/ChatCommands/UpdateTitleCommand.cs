namespace BotBrown.ChatCommands
{
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using System;
    using System.Threading.Tasks;

    public class UpdateTitleCommand : BaseChatCommand
    {
        private readonly IEventBus eventBus;

        public UpdateTitleCommand(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        public override UserType ElligableUserType => UserType.Editor;

        public override string[] Commands => new[] { "title" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            var channelUpdate = new UpdateChannelEvent
            {
                Title = chatCommandReceivedEvent.CommandArgs,
                Game = null
            };

            eventBus.Publish(channelUpdate);
            return Task.CompletedTask;
        }
    }
}
