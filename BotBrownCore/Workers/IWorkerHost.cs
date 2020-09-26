namespace BotBrown.Workers
{
    using System.Threading;
    using Castle.Windsor;

    public interface IWorkerHost
    {
        void Execute(CancellationToken cancellationToken, BotArguments botArguments);

        void PublishTTSMessage(string message);

        WindsorContainer Container { get; set; }
    }
}
