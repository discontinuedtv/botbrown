using System.ComponentModel;

namespace BotBrown.Configuration
{
    public interface IChangeableConfiguration : IConfiguration, INotifyPropertyChanged
    {
    }
}
