namespace BotBrownCore
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
        private FollowerService followerService;
        private readonly IList<Subscriber<NewFollowerEvent>> subscribers = new List<Subscriber<NewFollowerEvent>>();

        public void ConnectToTwitch(TwitchConfiguration twitchConfiguration)
        {
            TwitchAPI api = InitializeTwitchApi(twitchConfiguration);
            CreateFollowerService(twitchConfiguration, api);
            RegisterFollowerServiceCallback();
            followerService.Start();
        }

        public void Subscribe(Subscriber<NewFollowerEvent> subscriber)
        {
            subscribers.Add(subscriber);
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
                .Select(follow => new ChannelUser(follow.FromUserId, follow.FromUserName, follow.FromUserName))
                .ToList();

            NewFollowerEvent newFollowerEvent = new NewFollowerEvent(newFollowers);

            foreach (Subscriber<NewFollowerEvent> subscriber in subscribers)
            {
                subscriber.Notify(newFollowerEvent);
            }
        }
    }
}
