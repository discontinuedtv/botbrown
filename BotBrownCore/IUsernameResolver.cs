namespace BotBrown
{
    public interface IUsernameResolver
    {
        ChannelUser ResolveUsername(ChannelUser channelUser);
    }
}