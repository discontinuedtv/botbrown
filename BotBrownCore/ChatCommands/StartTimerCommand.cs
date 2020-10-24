namespace BotBrown.ChatCommands
{
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using BotBrown.Workers;
    using BotBrown.Workers.Timers;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class StartTimerCommand : BaseChatCommand
    {
        private readonly ITimerStore timerStore;
        private readonly IEventBus eventBus;

        public StartTimerCommand(ITimerStore timerStore, IEventBus eventBus)
        {
            this.timerStore = timerStore;
            this.eventBus = eventBus;
        }

        public override UserType ElligableUserType => UserType.Editor;

        public override string[] Commands => new[] { "timer" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            string[] parameters = chatCommandReceivedEvent.CommandArgs.Split(' ');
            if (parameters.Length < 2)
            {
                return Task.CompletedTask;
            }

            if (!int.TryParse(parameters[0], out int timeInSeconds))
            {
                return Task.CompletedTask;
            }

            string timerName = string.Join(" ", parameters.Skip(1));

            eventBus.Publish(new TextToSpeechEvent(chatCommandReceivedEvent.User, $"Der Timer {timerName} wurde gestartet."));

            TimeSpan timerLength = TimeSpan.FromSeconds(timeInSeconds);

            DateTime now = DateTime.Now;
            DateTime doneAt = now.Add(timerLength);
            TimerCommand timer = new TimerCommand(timerName, doneAt, new DefaultTimeProvider());
            timerStore.AddTimer(timer);

            Task.Delay(timerLength).ContinueWith(o => PublishTimeEnded(timer, chatCommandReceivedEvent.User));
            return Task.CompletedTask;
        }

        private void PublishTimeEnded(TimerCommand timer, ChannelUser user)
        {
            eventBus.Publish(new TextToSpeechEvent(user, $"Der Timer {timer.Name} ist abgelaufen."));
            timerStore.DeleteTimer(timer);
        }
    }
}
