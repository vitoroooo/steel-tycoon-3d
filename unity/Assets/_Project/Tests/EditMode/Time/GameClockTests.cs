using System;
using System.Collections.Generic;
using NUnit.Framework;
using Steel.Core.Time;

namespace Steel.Tests.Time
{
    public sealed class GameClockTests
    {
        private GameClock clock;

        [SetUp]
        public void SetUp() => clock = new GameClock(new TimeConfig(TestConfig.Read("Time")));

        [Test]
        public void TickOccursAtTenRealMinutesAndCarriesRemainder()
        {
            var ticks = new List<long>();
            clock.HourTick += ticks.Add;
            clock.Advance(599);
            Assert.AreEqual(0, ticks.Count);
            clock.Advance(1);
            clock.Advance(1500);
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, ticks);
            Assert.AreEqual(3.5, clock.GameHours);
            Assert.AreEqual(2100, clock.RealSeconds);
        }

        [Test]
        public void OfflineCatchUpCapsAtEightRealHours()
        {
            int ticks = 0;
            clock.HourTick += _ => ticks++;
            Assert.AreEqual(28800, clock.CatchUp(TimeSpan.FromDays(3).TotalSeconds));
            Assert.AreEqual(48, clock.GameHours);
            Assert.AreEqual(48, ticks);
        }

        [Test]
        public void ShortOfflineAndOnlineAdvanceAreEquivalent()
        {
            var other = new GameClock(new TimeConfig(TestConfig.Read("Time")));
            clock.CatchUp(1250);
            other.Advance(1250);
            Assert.AreEqual(other.GameHours, clock.GameHours);
            Assert.AreEqual(0, clock.CatchUp(0));
        }

        [TestCase(-1)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void InvalidElapsedDoesNotAdvance(double seconds)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => clock.Advance(seconds));
            Assert.Throws<ArgumentOutOfRangeException>(() => clock.CatchUp(seconds));
            Assert.AreEqual(0, clock.RealSeconds);
        }

        [Test]
        public void TickSubscribersSeeBoundaryTime()
        {
            var times = new List<double>();
            clock.HourTick += _ => times.Add(clock.RealSeconds);
            clock.Advance(1801);
            CollectionAssert.AreEqual(new double[] { 600, 1200, 1800 }, times);
        }
    }
}
