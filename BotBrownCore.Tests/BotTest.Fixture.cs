namespace BotBrownCore.Tests
{
    using BotBrown;
    using BotBrown.Configuration;
    using BotBrown.Workers.TextToSpeech;
    using BotBrown.Workers.Twitch;
    using Moq;

    public sealed partial class BotTest
    {
        private class Fixture
        {
            private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Strict);
            private Mock<ITwitchApiWrapper> apiWrapperMock;
            private Mock<ITwitchClientWrapper> clientWrapperMock;
            private Mock<IConfigurationManager> configurationManagerMock;
            private Mock<IEventBus> eventBusMock;
            private Mock<ITextToSpeechProcessor> textToSpeechProcessorMock;

            public Fixture()
            {
                apiWrapperMock = mockRepository.Create<ITwitchApiWrapper>();
                clientWrapperMock = mockRepository.Create<ITwitchClientWrapper>();
                configurationManagerMock = mockRepository.Create<IConfigurationManager>();
                eventBusMock = mockRepository.Create<IEventBus>();
                textToSpeechProcessorMock = mockRepository.Create<ITextToSpeechProcessor>();
            }

            internal Bot CreateTestObject()
            {
                return new Bot();
            }

            internal void PrepareTwitchApiThrowsExceptionOnConnect()
            {

            }
        }
    }
}
