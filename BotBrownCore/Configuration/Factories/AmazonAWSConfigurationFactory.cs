namespace BotBrown.Configuration.Factories
{
    public class AmazonAWSConfigurationFactory : IConfigurationFileFactory<AmazonAWSConfiguration>
    {
        public AmazonAWSConfiguration CreateDefaultConfiguration()
        {
            return new AmazonAWSConfiguration
            {
                AWSAccessKeyId = "",
                AWSSecretKey = ""
            };
        }
    }
}
