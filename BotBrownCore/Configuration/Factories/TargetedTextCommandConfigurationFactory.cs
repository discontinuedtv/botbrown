namespace BotBrown.Configuration.Factories
{
    public class TargetedTextCommandConfigurationFactory : IConfigurationFileFactory<TargetedTextCommandConfiguration>
    {
        public TargetedTextCommandConfiguration CreateDefaultConfiguration()
        {
            return new TargetedTextCommandConfiguration
            {
                Commands = new System.Collections.Generic.Dictionary<string, string>
                {
                    {
                       "liebe $1","$me schenkt $1 ein wenig Liebe"
                    }
                }
            };
        }
    }
}
