using NUnit.Framework;

namespace BotBrown
{
    [TestFixture]
    public sealed partial class UsernameResolverTest
    {
        private Fixture fixture = new Fixture();

        [Test]
        public void ResolveUsername_UsernameNotCached_GivenUsernameWithUnderscoreAndNumbers_ShouldResolveNameProperlyAndChangeConfiguration()
        {
            bool configurationChanged = false;
            ChannelUser user = fixture.PrepareResolve((sender, eventArgs) => { configurationChanged = true; });
            UsernameResolver testObject = fixture.CreateTestObject();

            var result = testObject.ResolveUsername(user);

            Assert.That(result.RealUsername, Is.EqualTo(user.RealUsername));
            Assert.That(result.UserId, Is.EqualTo(user.UserId));
            Assert.That(result.Username, Is.EqualTo("Real name"));
            Assert.That(configurationChanged, Is.True);
        }

        [Test]
        public void ResolveUsername_UsernameCached_ShouldReturnCachedConfiguration()
        {
            bool configurationChanged = false;
            ChannelUser user = fixture.PrepareCachedResolve((sender, eventArgs) => { configurationChanged = true; });
            UsernameResolver testObject = fixture.CreateTestObject();

            var result = testObject.ResolveUsername(user);

            Assert.That(result.RealUsername, Is.EqualTo(user.RealUsername));
            Assert.That(result.UserId, Is.EqualTo(user.UserId));
            Assert.That(result.Username, Is.EqualTo("Resolved Name"));
            Assert.That(configurationChanged, Is.False);
        }

        [Test]
        public void asd()
        {
            int value = 80;

            float result = value / 100;
        }
    }
}
