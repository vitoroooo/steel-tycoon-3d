using System;
using NUnit.Framework;
using Steel.Core.Crew;

namespace Steel.Tests.Crew
{
    public sealed class CrewOperationTests : CrewFixture
    {
        [Test] public void ShiftAndFatigueChangeActualLoadAndRejectedLoadsDoNotTireCrew()
        {
            Game.SetCrewPolicy(ShiftPolicy.Hemat);
            Receive(5); Assert.Zero(Game.Yard.stock[1]);
            Game.Yard.bids[1] = 0;
            Receive(6); Game.CrewHour(7);
            Assert.AreEqual(100, Game.Crew.dayStamina);
            Game.Yard.bids[1] = 1;
            for (int h = 7; h < 15; h++) { Receive(h); Game.CrewHour(h + 1); }
            Assert.AreEqual(800, Game.Yard.stock[1]); Assert.AreEqual(20, Game.Crew.dayStamina);
            Assert.AreEqual(100, Game.Crew.nightStamina);
            Receive(15); Assert.AreEqual(870, Game.Yard.stock[1]);
            Game.CrewHour(16); Game.CrewHour(17); Assert.AreEqual(20, Game.Crew.dayStamina);
            Game.SetCrewPolicy(ShiftPolicy.Lembur); Receive(17);
            Assert.AreEqual(968, Game.Yard.stock[1], .00001);
        }

        [Test] public void OvertimePaysTwoOperatorsAndCostsMoraleOnPayday()
        {
            Game.SetCrewPolicy(ShiftPolicy.Lembur); Receive(16); Game.CrewHour(17);
            Assert.AreEqual(90, Game.Crew.dayStamina); Assert.AreEqual(90, Game.Crew.nightStamina);
            var before = Game.Cash; Game.CrewHour(24);
            Assert.AreEqual(before - 1100000, Game.Cash); Assert.AreEqual(95, Game.Crew.morale);
            Game.SetCrewPremium(true); Game.CrewHour(48);
            Assert.AreEqual(before - 2310000, Game.Cash, .001); Assert.AreEqual(100, Game.Crew.morale);
        }

        [Test] public void EconomyShiftFinishesUnloadingAfterNextDawn()
        {
            Game.SetCrewPolicy(ShiftPolicy.Hemat); Game.Yard.stock[1] = 15000;
            var net = Game.CrewShipGrade(1, Business, 4000, 17.5, 0, 100);
            Assert.AreEqual(31, Game.Yard.unloadHour);
            double before = Game.Cash; Game.PayShipment(30.9); Assert.AreEqual(before, Game.Cash);
            Game.PayShipment(31); Assert.AreEqual(before + net, Game.Cash);
        }

        [Test] public void StrikePausesReceiptUntilRestRestoresMorale()
        {
            Game.Crew.morale = 18;
            Receive(8); Assert.Zero(Game.Yard.stock[1]); Game.CrewHour(9); Receive(9);
            Assert.AreEqual(100, Game.Yard.stock[1]);
        }

        [Test] public void LegacyAndInvalidSavesAreDistinguished()
        {
            Game.RestoreCrew(null); Assert.AreEqual(100, Game.Crew.dayStamina);
            Game.RestoreCrew(new CrewSave { version = 0, morale = 0 });
            Assert.AreEqual(ShiftPolicy.Normal, Game.Crew.policy); Assert.AreEqual(100, Game.Crew.morale);
            Assert.Throws<FormatException>(() => Game.RestoreCrew(new CrewSave { morale = double.NaN }));
        }
    }
}
