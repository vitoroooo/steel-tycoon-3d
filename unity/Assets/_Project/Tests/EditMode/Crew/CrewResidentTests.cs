using System;
using NUnit.Framework;

namespace Steel.Tests.Crew
{
    public sealed class CrewResidentTests : CrewFixture
    {
        [Test] public void RealNightWorkAndRoadQueueTriggerBlockadeWhileCsrAndQuietHoursRecover()
        {
            Game.Crew.patience = 22; Receive(451); Game.Yard.stuck = true;
            Game.CrewHour(452); Assert.AreEqual(19, Game.Crew.patience);
            Assert.AreEqual(476, Game.Crew.blockadeUntil);
            Receive(452); Assert.AreEqual(100, Game.Yard.stock[1]);
            var before = Game.Cash; int rep = Game.Yard.reputation;
            Game.FundCrewCsr(452);
            Assert.AreEqual(before - 2000000, Game.Cash); Assert.AreEqual(rep + 1, Game.Yard.reputation);
            Assert.AreEqual(44, Game.Crew.patience);
            Assert.Throws<InvalidOperationException>(() => Game.FundCrewCsr(452));
            Game.Yard.stuck = false; Game.CrewHour(453);
            Assert.AreEqual(45, Game.Crew.patience);
            Receive(476); Assert.AreEqual(200, Game.Yard.stock[1]);
        }
        [Test] public void FactoryCoverReducesNightNoiseAndDustAndRfidOffsetsPressure()
        {
            Game.Crew.patience = 70;
            Game.CrewHour(452, 2, true); Assert.AreEqual(65, Game.Crew.patience);
            Game.CrewHour(453, 2, true, true, true); Assert.AreEqual(63, Game.Crew.patience);
        }
    }
}
