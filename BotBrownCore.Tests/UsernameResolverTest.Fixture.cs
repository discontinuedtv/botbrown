namespace BotBrown
{
    using BotBrown.Configuration;
    using Moq;
    using System;
    using System.ComponentModel;

    public sealed partial class UsernameResolverTest
    {
        private class Fixture : IDisposable
        {
            private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Strict);
            private readonly Mock<IConfigurationManager> configurationManagerMock;

            public Fixture()
            {
                configurationManagerMock = mockRepository.Create<IConfigurationManager>();
            }

            public UsernameResolver CreateTestObject()
            {
                return new UsernameResolver(configurationManagerMock.Object);
            }

            public ChannelUser PrepareResolve(PropertyChangedEventHandler configurationWasChangedCheck)
            {
                UsernameConfiguration configuration = new UsernameConfiguration();
                configuration.PropertyChanged += configurationWasChangedCheck;

                configurationManagerMock
                    .Setup(x => x.LoadConfiguration<UsernameConfiguration>())
                    .Returns(configuration);

                return CreateUserWithUnresolvedName();
            }

            public ChannelUser PrepareCachedResolve(PropertyChangedEventHandler configurationWasChangedCheck)
            {
                UsernameConfiguration configuration = new UsernameConfiguration();
                configuration.AddUsername(new ChannelUser("4129083", "Real123_name", "Resolved Name"));
                configuration.PropertyChanged += configurationWasChangedCheck;

                configurationManagerMock
                    .Setup(x => x.LoadConfiguration<UsernameConfiguration>())
                    .Returns(configuration);

                return CreateUserWithUnresolvedName();
            }

            public ChannelUser CreateUserWithUnresolvedName()
            {
                return new ChannelUser("4129083", "Real123_name", "Real123_name");
            }

            public void Dispose()
            {
                mockRepository.VerifyAll();
            }
        }
    }
}
