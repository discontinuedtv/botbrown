namespace BotBrownCore.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public sealed partial class BotTest
    {
        private Fixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new Fixture();
        }

        [Test]
        public void Execute_ShouldRefreshCommands()
        {
            fixture.PrepareTwitchApiThrowsExceptionOnConnect();
            Bot testObject = fixture.CreateTestObject();

            testObject.Execute();
        }
    }
}
