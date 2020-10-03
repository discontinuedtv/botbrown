namespace BotBrown.Configuration
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IConfigurationWatcher
    {
        Task<bool> StartWatch(CancellationToken cancellationToken);
    }
}