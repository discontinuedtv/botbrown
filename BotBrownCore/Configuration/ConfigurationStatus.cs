namespace BotBrown.Configuration
{
    public class ConfigurationStatus
    {
        public ConfigurationStatus(string filename, bool isValid)
        {
            Filename = filename;
            IsValid = isValid;
        }

        public string Filename { get; }

        public bool IsValid { get; }
    }
}