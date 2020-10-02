namespace BotBrown.Workers.Twitch
{
    using BotBrown;
    using BotBrown.Configuration;
    using BotBrown.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TwitchLib.Api;
    using TwitchLib.Api.Core;
    using TwitchLib.Api.Services;
    using TwitchLib.Api.Services.Events.FollowerService;
    using System.Threading.Tasks;
    using TwitchLib.Api.V5.Models.Channels;
    using Serilog;    

    public class TwitchApiWrapper : ITwitchApiWrapper
    {
        private readonly IUsernameResolver usernameResolver;
        private readonly IEventBus bus;
        private readonly IConfigurationManager configurationManager;
        private readonly ILogger logger;
        private FollowerService followerService;
        private TwitchAPI api;

        public TwitchApiWrapper(IUsernameResolver usernameResolver, IEventBus bus, IConfigurationManager configurationManager, ILogger logger)
        {
            this.usernameResolver = usernameResolver;
            this.bus = bus;
            this.configurationManager = configurationManager;
            this.logger = logger;
        }

        public void ConnectToTwitch(TwitchConfiguration twitchConfiguration)
        {
            SetUpEvents();

            api = InitializeTwitchApi(twitchConfiguration);
            CreateFollowerService(twitchConfiguration, api);
            RegisterFollowerServiceCallback();
            followerService.Start();
        }

        private void SetUpEvents()
        {
            bus.AddTopic<NewFollowerEvent>();
        }

        public void Stop()
        {
            followerService?.Stop();
        }

        private TwitchAPI InitializeTwitchApi(TwitchConfiguration twitchConfiguration)
        {
            return new TwitchAPI(null, null, new ApiSettings
            {
                ClientId = twitchConfiguration.ApiClientId,
                AccessToken = twitchConfiguration.ApiAccessToken
            });
        }

        private void CreateFollowerService(TwitchConfiguration twitchConfiguration, TwitchAPI api)
        {
            followerService = new FollowerService(api, 10);
            followerService.SetChannelsByName(new List<string> { twitchConfiguration.Username }); // TODO ändern!
        }

        private void RegisterFollowerServiceCallback()
        {
            followerService.OnNewFollowersDetected += Api_OnFollowerDetected;
        }

        private void Api_OnFollowerDetected(object sender, OnNewFollowersDetectedArgs newFollowerDetectedEventArguments)
        {
            DateTime dateToCheckAgainst = DateTime.UtcNow.AddSeconds(-90);

            List<ChannelUser> newFollowers = newFollowerDetectedEventArguments.NewFollowers
                .Where(follow => follow.FollowedAt >= dateToCheckAgainst)
                .Select(follow => usernameResolver.ResolveUsername(new ChannelUser(follow.FromUserId, follow.FromUserName, follow.FromUserName)))
                .ToList();

            NewFollowerEvent newFollowerEvent = new NewFollowerEvent(newFollowers);
            bus.Publish(newFollowerEvent);
        }

        public void UpdateChannel(UpdateChannelEvent updateChannelEvent)
        {
            TwitchConfiguration twitchConfiguration = configurationManager.LoadConfiguration<TwitchConfiguration>(ConfigurationFileConstants.Twitch);
            api.V5.Channels.GetChannelByIDAsync(twitchConfiguration.BroadcasterUserId)
                .ContinueWith(task =>
                {
                    updateChannelEvent.Update(task.Result);
                    return api.V5.Channels.UpdateChannelAsync(twitchConfiguration.BroadcasterUserId, updateChannelEvent.Title, updateChannelEvent.Game);
                })
                .ContinueWith(task => task.Result.ContinueWith(x => PublishSuccessMessageOnCompletion(x)));
        }

        public async Task<string> GetCurrentGame()
        {
            TwitchConfiguration twitchConfiguration = configurationManager.LoadConfiguration<TwitchConfiguration>(ConfigurationFileConstants.Twitch);
            var channel = await api.V5.Channels.GetChannelByIDAsync(twitchConfiguration.BroadcasterUserId);
            return channel.Game;
        }

        private void PublishSuccessMessageOnCompletion(Task<Channel> task)
        {
            task.ContinueWith(t =>
            {
                PublishFeedbackMessage("Titel oder Game konnte nicht geändert werden.");
                logger.Error("Beim ändern der Kanaleinstellungen ist ein Fehler aufgetreten: {Exception}", t.Exception);
            },
            TaskContinuationOptions.OnlyOnFaulted);

            task.ContinueWith(t =>
            {
                PublishFeedbackMessage("Titel oder Game wurden geändert.");
            },
            TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private void PublishFeedbackMessage(string message)
        {
            TwitchConfiguration twitchConfiguration = configurationManager.LoadConfiguration<TwitchConfiguration>(ConfigurationFileConstants.Twitch);
            bus.Publish(new SendChannelMessageRequestedEvent(message, twitchConfiguration.Channel));
        }        
    }
}
