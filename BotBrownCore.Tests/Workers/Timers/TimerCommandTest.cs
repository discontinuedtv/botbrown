using BotBrown.Workers.Timers;
using NUnit.Framework;
using System;

namespace BotBrownCore.Tests.Workers.Timers
{
    public sealed partial class TimerCommandTest
    {
        private readonly Fixture fixture = new Fixture();

        [Test]
        public void FormattedTimeLeft_TimerWithFifteenSecondsLeft_ShouldReturnSecondsPart()
        {
            DateTime referenceTime = fixture.PrepareReferenceTime();
            TimerCommand testObject = fixture.CreateTestObjectWith("Test", referenceTime.AddSeconds(15));

            string timeLeft = testObject.FormattedTimeLeft;

            Assert.That(timeLeft, Is.EqualTo("15 Sek"));
        }

        [Test]
        public void FormattedTimeLeft_TimerWithOneSecondLeft_ShouldUseSingular()
        {
            DateTime referenceTime = fixture.PrepareReferenceTime();
            TimerCommand testObject = fixture.CreateTestObjectWith("Test", referenceTime.AddSeconds(1));

            string timeLeft = testObject.FormattedTimeLeft;

            Assert.That(timeLeft, Is.EqualTo("1 Sek"));
        }

        [Test]
        public void FormattedTimeLeft_TimerWithTwoMinutesAndTwentySecondsLeft_ShouldReturnMinutesAndSecondsPart()
        {
            DateTime referenceTime = fixture.PrepareReferenceTime();
            TimerCommand testObject = fixture.CreateTestObjectWith("Test", referenceTime.AddMinutes(2).AddSeconds(20));

            string timeLeft = testObject.FormattedTimeLeft;

            Assert.That(timeLeft, Is.EqualTo("2 Min 20 Sek"));
        }

        [Test]
        public void FormattedTimeLeft_TimerWithOneMinuteLeft_ShouldReturnSingularMinutePart()
        {
            DateTime referenceTime = fixture.PrepareReferenceTime();
            TimerCommand testObject = fixture.CreateTestObjectWith("Test", referenceTime.AddMinutes(1));

            string timeLeft = testObject.FormattedTimeLeft;

            Assert.That(timeLeft, Is.EqualTo("1 Min"));
        }

        [Test]
        public void FormattedTimeLeft_TimerWithTwelveHoursTenMinutesAndElevenSecondsLeft_ShouldReturnHoursMinutesAndSecondsPart()
        {
            DateTime referenceTime = fixture.PrepareReferenceTime();
            TimerCommand testObject = fixture.CreateTestObjectWith("Test", referenceTime.AddHours(12).AddMinutes(10).AddSeconds(11));

            string timeLeft = testObject.FormattedTimeLeft;

            Assert.That(timeLeft, Is.EqualTo("12 Std 10 Min 11 Sek"));
        }

        [Test]
        public void FormattedTimeLeft_TimerWithOneHourThirtyTwoMinutesAndOneSecondLeft_ShouldReturnHoursMinutesAndSecondsPart()
        {
            DateTime referenceTime = fixture.PrepareReferenceTime();
            TimerCommand testObject = fixture.CreateTestObjectWith("Test", referenceTime.AddHours(1).AddMinutes(32).AddSeconds(1));

            string timeLeft = testObject.FormattedTimeLeft;

            Assert.That(timeLeft, Is.EqualTo("1 Std 32 Min 1 Sek"));
        }
    }
}
