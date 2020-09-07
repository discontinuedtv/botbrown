using System.Threading;

namespace BotBrown.Workers
{
    public interface IWorkerHost
    {
        void Execute(CancellationToken cancellationToken, BotArguments botArguments);

        void PublishTTSMessage(string message);

        void PublishSoundCommand(string message);
    }
}
