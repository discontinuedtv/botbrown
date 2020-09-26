namespace BotBrown.Configuration
{
    using System;

    public class ConfigurationFileAttribute : Attribute
    {
        public ConfigurationFileAttribute(string filename)
        {
            Filename = filename;
        }

        public string Filename { get; }
    }
}
