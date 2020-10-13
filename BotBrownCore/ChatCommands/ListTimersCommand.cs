namespace BotBrown.ChatCommands
{
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using BotBrown.Workers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ListTimersCommand : BaseChatCommand
    {
        private readonly ITimerStore timerStore;
        private readonly IEventBus eventBus;

        public ListTimersCommand(ITimerStore timerStore, IEventBus eventBus)
        {
            this.timerStore = timerStore;
            this.eventBus = eventBus;
        }

        public override UserType ElligableUserType => UserType.Editor;

        public override string[] Commands => new[] { "timers" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            var timerCommands = timerStore.GetAllTimers();

            if (!timerCommands.Any())
            {
                eventBus.Publish(new SendChannelMessageRequestedEvent($"Es sind keine Timer aktiv!", chatCommandReceivedEvent.ChannelName));
                return Task.CompletedTask;
            }

            List<string> expiringTimers = new List<string>();
            foreach (var timer in timerCommands)
            {
                expiringTimers.Add($"{timer.Name}: {timer.FormattedTimeLeft}");
            }

            string expiringTimerOutput = string.Join(", ", expiringTimers);
            eventBus.Publish(new SendChannelMessageRequestedEvent($"Folgende Timer sind aktiv -> {expiringTimerOutput}", chatCommandReceivedEvent.ChannelName));
            return Task.CompletedTask;
        }
    }
}
