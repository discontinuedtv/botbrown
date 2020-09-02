using BotBrown.Configuration;

namespace BotBrown
{
    public interface IUsernameResolver
    {
        ChannelUser ResolveUsername(ChannelUser channelUser);
    }
}