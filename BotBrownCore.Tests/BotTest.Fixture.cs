namespace BotBrownCore.Tests
{
    using BotBrownCore.Configuration;
    using Moq;

    public sealed partial class BotTest
    {
        private class Fixture
        {
            private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Strict);
            private Mock<ITwitchApiWrapper> apiWrapperMock;
            private Mock<ITwitchClientWrapper> clientWrapperMock;
            private Mock<IConfigurationManager> configurationManagerMock;

            public Fixture()
            {
                apiWrapperMock = mockRepository.Create<ITwitchApiWrapper>();
                clientWrapperMock = mockRepository.Create<ITwitchClientWrapper>();
                configurationManagerMock = mockRepository.Create<IConfigurationManager>();
            }

            internal Bot CreateTestObject()
            {
                return new Bot(apiWrapperMock.Object, clientWrapperMock.Object, configurationManagerMock.Object);
            }

            internal void PrepareTwitchApiThrowsExceptionOnConnect()
            {

            }
        }
    }
}
