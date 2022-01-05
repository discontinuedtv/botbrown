namespace BotBrown.ChannelData
{
    using System;

    public class ChannelDataStorageAttribute : Attribute
    {
        public ChannelDataStorageAttribute(string endpointName)
        {
            EndpointName = endpointName;
        }

        public string EndpointName { get; }
    }
}
