namespace BotBrown.Configuration.Transient
{
    public interface ITransientConfigStore
    {
        bool Store<T>(T configToStore);

        bool Get<T>(out T t);

        bool Clear<T>();
    }
}
