using System;
using NUnit.Framework;
using Steel.Core.Time;

namespace Steel.Tests.Crew
{
    public sealed class CrewOfflineTests : CrewFixture
    {
        [TestCase(false, 28800, 48)] [TestCase(true, 43200, 72)]
        public void OfflineWorkUsesCappedHoursAndNeverUsesUnsafeTools(bool senior, int seconds, int hours)
        {
            if (senior) Game.PromoteForeman();
            var time = new TimeConfig(TestConfig.Read("Time"));
            var clock = new GameClock(time);
            clock.Restore(500 * 600);
            clock.HourTick += hour => {
                Game.CrewHour(hour);
                Receive(hour, 5);
                Game.CrewEmergencyBreakdown(hour);
                Game.CrewConfirmedAccident(hour);
            };
            Assert.AreEqual(seconds, Game.CrewCatchUp(90000, time.OfflineCapRealSeconds, clock.Advance));
            Assert.AreEqual(500 + hours, clock.GameHours);
            Assert.Zero(Game.Crew.incidents); Assert.Zero(Game.Crew.riskSequence);
            Assert.Zero(Game.Yard.stock[1]); Assert.IsFalse(Game.CrewOffline);
            clock.HourTick += hour => Receive(hour);
            Game.CrewCatchUp(600, time.OfflineCapRealSeconds, clock.Advance);
            Assert.AreEqual(100, Game.Yard.stock[1]);
        }

        [Test] public void OfflineFlagClearsIfCatchUpFails()
        {
            Assert.Throws<InvalidOperationException>(() =>
                Game.CrewCatchUp(2, 28800, sec => throw new InvalidOperationException()));
            Assert.IsFalse(Game.CrewOffline);
        }
    }
}
