namespace BotBrownCore.Tests.Workers.Timers
{
    using BotBrown;
    using BotBrown.Workers.Timers;
    using Moq;
    using System;

    public sealed partial class TimerCommandTest
    {
        private class Fixture
        {
            private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Strict);
            private readonly Mock<ITimeProvider> timeProviderMock;

            public Fixture()
            {
                timeProviderMock = mockRepository.Create<ITimeProvider>();
            }

            public TimerCommand CreateTestObjectWith(string timerName, DateTime doneAt)
            {
                return new TimerCommand(timerName, doneAt, timeProviderMock.Object);
            }

            internal DateTime PrepareReferenceTime()
            {
                DateTime referenceTime = new DateTime(2020, 03, 29, 12, 32, 44);

                timeProviderMock.
                    SetupGet(x => x.Now)
                    .Returns(referenceTime);

                return referenceTime;
            }
        }
    }
}
