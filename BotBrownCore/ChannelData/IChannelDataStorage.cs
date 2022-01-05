namespace BotBrown.ChannelData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection;
    using Grpc.Net.Client;
    using BotBrown.Configuration;
    using System.Collections.Concurrent;

    internal interface IChannelDataStorage
    {
        void ResetCacheFor(string filename);

        T LoadFromStorage<T>()
            where T : IChannelData;

        void WriteChannelData(IChannelData channelData);

        IEnumerable<IChannelData> CheckChannelDataStatus();
    }

    public interface IChannelData
    {

    }

    public class RemoteChannelDataStorage
    {
        private readonly ConcurrentDictionary<Type, IConfiguration> storages;

        public RemoteChannelDataStorage()
        {

        }

    }

    public class BirthdayClient : IDisposable
    {
        private GrpcChannel channel;
        private BirthdayService.BirthdayServiceClient client;

        public BirthdayClient(string hostname)
        {
            channel = GrpcChannel.ForAddress(hostname);
            client = new BirthdayService.BirthdayServiceClient(channel);
        }

        public async Task AddBirthday(Birthday birthday)
        {
            await client.StoreBirthdayAsync(new StoreBirthdayRequest { UserId = birthday.UserId, Day = birthday.Day.Ticks });
        }

        public async Task MarkAsCongratulated(string userId, int year)
        {
            await client.MarkCongratulatedAsync(new MarkCongratulatedRequest { UserId = userId, Year = year });
        }

        public void Dispose()
        {
            channel?.Dispose();
        }
    }

    /*
    public class RemoteChannelDataStorage : IChannelDataStorage
    {
        private string host;
        private string accessToken;

        private readonly IConfigurationPathProvider configurationPathProvider;
        private readonly IConfigurationFileFactoryRegistry registry;
        private readonly ILogger logger;
        private readonly ConcurrentDictionary<Type, IConfiguration> storages;

        public RemoteChannelDataStorage(string host, string accessToken)
        {
            this.host = host;
            this.accessToken = accessToken;
        }

        public IEnumerable<IChannelData> CheckChannelDataStatus()
        {
            throw new NotImplementedException();
        }

        public T LoadFromStorage<T>() where T : IChannelData
        {
            Type key = typeof(T);
            if (storages.ContainsKey(key))
            {
                return (T)storages[key];
            }

            string endpointName = key.GetCustomAttribute<ChannelDataStorageAttribute>().EndpointName;
            
            // Access Remote Endpoint to load data

            if (!File.Exists(pathToFile))
            {
                IConfigurationFileFactory<T> factory = registry.GetFactory<T>();
                T defaultConfiguration = factory.CreateDefaultConfiguration();
                WriteConfiguration(defaultConfiguration);

                storages.TryAdd(key, defaultConfiguration);

                if (defaultConfiguration is IChangeableConfiguration changeableConfiguration)
                    changeableConfiguration.PropertyChanged += ConfigurationChanged;

                return defaultConfiguration;
            }

            using (TextReader reader = new StreamReader($"{ConfigurationBasePath}/{endpointName}"))
            {
                string serialzedConfiguration = reader.ReadToEnd();
                var configuration = JsonConvert.DeserializeObject<T>(serialzedConfiguration);
                storages.TryAdd(key, configuration);

                if (configuration is IChangeableConfiguration changeableConfiguration)
                    changeableConfiguration.PropertyChanged += ConfigurationChanged;

                return configuration;
            }
        }

        public void ResetCacheFor(string filename)
        {
            throw new NotImplementedException();
        }

        public void WriteChannelData(IChannelData channelData)
        {
            
        }
    }

    public partial class BirthdaysChannelData : IChannelData
    {
        [JsonConverter(typeof(CustomDictionaryConverter<DayMonth, List<Birthday>>))]
        public Dictionary<DayMonth, List<Birthday>> Birthdays { get; set; } = new Dictionary<DayMonth, List<Birthday>>();

        public event PropertyChangedEventHandler PropertyChanged;

        public bool ContainsBirthdayForDate(DateTime dateToCheck)
        {
            var dayMonth = new DayMonth { Day = dateToCheck.Day, Month = dateToCheck.Month };
            return Birthdays.ContainsKey(dayMonth);
        }

        public void AddBirthday(DateTime birthday, string userId)
        {
            Birthday item = new Birthday { Day = birthday, Gratulated = new List<int>(), UserId = userId };
            DayMonth? dayMonth = new DayMonth { Day = birthday.Day, Month = birthday.Month };
            if (Birthdays.ContainsKey(dayMonth))
            {
                Birthdays[dayMonth].Add(item);
            }
            else
            {
                Birthdays[dayMonth] = new List<Birthday> { item };
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Birthdays)));
        }

        public void DeleteBirthday(string userId)
        {
            foreach (var key in Birthdays.Keys.ToList())
            {
                Birthdays[key] = Birthdays[key].Where(x => x.UserId != userId).ToList();
                if (!Birthdays[key].Any())
                {
                    Birthdays.Remove(key);
                }
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Birthdays)));
        }

        public bool IsValid()
        {
            return true;
        }

        public List<Birthday> GetBirthdays(DateTime date)
        {
            var dayMonth = new DayMonth { Day = date.Day, Month = date.Month };
            return Birthdays[dayMonth];
        }

        public void MarkChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BirthdaysConfiguration)));
        }
    }
    */
}
