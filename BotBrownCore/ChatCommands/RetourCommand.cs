﻿namespace BotBrown.ChatCommands
{
    using BotBrown.Configuration;
    using BotBrown.Events;
    using BotBrown.Events.Twitch;
    using BotBrown.Models;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// A command that the user can use to show in the chat that she/he has returned.
    /// </summary>
    public class RetourCommand : BaseChatCommand
    {
        private readonly IEventBus eventBus;
        private readonly Random random;

        public RetourCommand(IEventBus eventBus)
        {
            this.eventBus = eventBus;
            random = new Random();
        }

        public override UserType ElligableUserType => UserType.All;

        public override string[] Commands => new[] { "re" };

        public override TimeSpan Cooldown => TimeSpan.Zero;

        public override bool ShouldContinue => false;

        public override Task ConsumeCommandSpecific(ChatCommandReceivedEvent chatCommandReceivedEvent)
        {
            // TODO: Vielleicht lieber Zurück aus der Zukunft Daten? 1955, 1985, 2015
            ChannelUser user = chatCommandReceivedEvent.User;
            string channelName = chatCommandReceivedEvent.ChannelName;

            int year = random.Next(1400, 2180);
            eventBus.Publish(new SendChannelMessageRequestedEvent($"{user.RealUsername} ist zurück von der Zeitreise aus dem Jahr {year}", channelName));

            return Task.CompletedTask;
        }
    }
}
