namespace BotBrown.Configuration
{
    using System.ComponentModel;

    [ConfigurationFile(ConfigurationFileConstants.Amazon)]
    public class AmazonAWSConfiguration : IChangeableConfiguration
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string AWSAccessKeyId { get; set; }

        public string AWSSecretKey { get; set; }

        public void SetAWSSecrets(string accesskey, string secretkey)
        {
            AWSAccessKeyId = accesskey;
            AWSSecretKey = secretkey;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AmazonAWSConfiguration)));
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(AWSAccessKeyId) && !string.IsNullOrEmpty(AWSSecretKey);
        }
    }
}
