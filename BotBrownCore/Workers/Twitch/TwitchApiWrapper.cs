namespace BotBrownCore.Workers.Twitch
{
    using BotBrownCore.Configuration;
    using BotBrownCore.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TwitchLib.Api;
    using TwitchLib.Api.Core;
    using TwitchLib.Api.Services;
    using TwitchLib.Api.Services.Events.FollowerService;

    public class TwitchApiWrapper : ITwitchApiWrapper
    {
        private readonly IUsernameResolver usernameResolver;
        private readonly IEventBus bus;
        private FollowerService followerService;

        public TwitchApiWrapper(IUsernameResolver usernameResolver, IEventBus bus)
        {
            this.usernameResolver = usernameResolver;
            this.bus = bus;
        }

        public void ConnectToTwitch(TwitchConfiguration twitchConfiguration)
        {
            SetUpEvents();

            TwitchAPI api = InitializeTwitchApi(twitchConfiguration);
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
            followerService.Stop();
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
    }
}
