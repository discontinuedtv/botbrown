namespace BotBrown.Configuration.Transient
{
    using System;
    using System.Collections.Generic;

    public class TransientConfigStore : ITransientConfigStore
    {
        private Dictionary<Type, object> transientConfigs = new Dictionary<Type, object>();

        public bool Clear<T>()
        {
            return transientConfigs.Remove(typeof(T));
        }

        public bool Get<T>(out T t)
        {
            if (transientConfigs.ContainsKey(typeof(T)))
            {
                t = (T)transientConfigs[typeof(T)];
                return true;
            }

            t = default;
            return false;
        }

        public bool Store<T>(T configToStore)
        {
            transientConfigs[typeof(T)] = configToStore;
            return true;
        }
    }
}
