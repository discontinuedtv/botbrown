namespace BotBrown.ChatCommands
{
    using BotBrown.Configuration.Transient;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using System;
    using System.Threading.Tasks;

    public class MuteCommand : BaseChatCommand
    {
        private readonly ITransientConfigStore transientConfigStore;
        private readonly IEventBus eventBus;

        public override UserType ElligableUserType => UserType.Editor;

        public override string[] Commands => new[] { "mute" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => true;

        public MuteCommand(ITransientConfigStore transientConfigStore, IEventBus eventBus)
        {
            this.transientConfigStore = transientConfigStore;
            this.eventBus = eventBus;
        }

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            var args = chatCommandReceivedEvent.CommandArgs.Split(' ');
            if (args.Length == 0)
            {
                transientConfigStore.Store(new MuteTimer());
                eventBus.Publish(new SendChannelMessageRequestedEvent("Bot für 10 Minuten gemutet.", chatCommandReceivedEvent.ChannelName));
                return Task.CompletedTask;
            }

            if (int.TryParse(args[0], out var time))
            {
                transientConfigStore.Store(new MuteTimer(time));
                eventBus.Publish(new SendChannelMessageRequestedEvent($"Bot für {time} Minuten gemutet.", chatCommandReceivedEvent.ChannelName));
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
