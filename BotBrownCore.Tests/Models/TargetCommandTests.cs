namespace BotBrown.Models
{
    using NUnit.Framework;

    [TestFixture]
    public class TargetCommandTests
    {
        [Test]
        public void Text_WithOnlyMe_ShouldReplaceText()
        {
            var targetCommand = new TargetCommand(null, "$me ist cool", "steve");

            Assert.That(targetCommand.IsValid(), Is.True);
            Assert.That(targetCommand.ResolvePreparedText(), Is.EqualTo("steve ist cool"));
        }

        [Test]
        public void Text_WithMeAndOneArgs_ShouldReplaceText()
        {
            var targetCommand = new TargetCommand("discontinuedman", "$me mag $1", "steve");

            Assert.That(targetCommand.IsValid(), Is.True);
            Assert.That(targetCommand.ResolvePreparedText(), Is.EqualTo("steve mag discontinuedman"));
        }

        [Test]
        public void Text_WithTwoArgs_ShouldCatchAllAndReplaceText()
        {
            var targetCommand = new TargetCommand("discontinuedman und icequeenmairim", "$me mag $1", "steve");

            Assert.That(targetCommand.IsValid(), Is.True);
            Assert.That(targetCommand.ResolvePreparedText(), Is.EqualTo("steve mag discontinuedman und icequeenmairim"));
        }

        [Test]
        public void Text_WithThreeArgs_ShouldCatchAllAndReplaceText()
        {
            var targetCommand = new TargetCommand("Käse Pizza und Brot backen", "$me mag $1 und liebt $2", "steve");

            Assert.That(targetCommand.IsValid(), Is.True);
            Assert.That(targetCommand.ResolvePreparedText(), Is.EqualTo("steve mag Käse und liebt Pizza und Brot backen"));
        }

        [Test]
        public void Text_WithInvalidArgs_ShouldBeNotValid()
        {
            var targetCommand = new TargetCommand(null, "$1 ist cool", "steve");

            Assert.That(targetCommand.IsValid(), Is.False);
        }
    }
}
