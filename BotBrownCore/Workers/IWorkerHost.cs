using System.Threading;

namespace BotBrown.Workers
{
    public interface IWorkerHost
    {
        void Execute(CancellationToken cancellationToken);
    }
}
