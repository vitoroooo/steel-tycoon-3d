using System;
using NUnit.Framework;
using Steel.Core.Crew;

namespace Steel.Tests.Crew
{
    public sealed class CrewSafetyTests : CrewFixture
    {
        [Test] public void LowConditionOnAcceptedLoadInvestigatesOnceAndOperationsRecoverAfter24Hours()
        {
            var c = Game.Crew;
            c.riskSequence = 7; // Next deterministic sample is 0.107 < 0.12.
            Receive(435, 19);
            Assert.AreEqual(100, Game.Yard.stock[1]); Assert.AreEqual(1, c.incidents);
            Assert.AreEqual(70, c.morale); Assert.AreEqual(21, Game.Yard.reputation);
            Assert.AreEqual(100000000 - 100 - 25000000, Game.Cash);
            Game.CrewConfirmedAccident(436); Assert.AreEqual(1, c.incidents);
            Game.Yard.stock[1] = 15000;
            Assert.Throws<InvalidOperationException>(() => Game.CrewShipGrade(1, Business, 4000, 458.99, 0, 100));
            Receive(458); Assert.AreEqual(15000, Game.Yard.stock[1]);
            Game.CreditCrewSaleReputation(100);
            Assert.AreEqual(22, Game.Yard.reputation);
            Assert.Greater(Game.CrewShipGrade(1, Business, 4000, 459, 0, 100), 0);
            Assert.Zero(Game.Yard.stock[1]);
        }

        [Test] public void BreakdownRollProtectionEventBudgetAndDeferredPayment()
        {
            Game.Yard.stock[1] = 15000;
            Game.CrewShipGrade(1, Business, 4000, 432, 0, 100);
            var due = Game.Yard.unloadHour;
            Game.CrewEmergencyBreakdown(432); Assert.Zero(Game.Crew.incidents);
            Game.CrewEmergencyBreakdown(432);
            Assert.AreEqual(1, Game.Crew.incidents); Assert.AreEqual(due + 24, Game.Yard.unloadHour);
            Game.CrewConfirmedAccident(456); Game.CrewConfirmedAccident(480);
            Assert.AreEqual(2, Game.Crew.incidents);
            Game.CrewConfirmedAccident(576); Assert.AreEqual(3, Game.Crew.incidents);
        }

        [Test] public void NoIncidentFromIdleTicksRejectedLoadsOrNewcomerOperations()
        {
            Game.Crew.riskSequence = 7;
            Receive(100, 5);
            Game.Yard.bids[1] = 0; Receive(500, 5); Game.CrewHour(501);
            Game.SetCrewPolicy(ShiftPolicy.Lembur); Game.CrewHour(502);
            Assert.Zero(Game.Crew.incidents); Assert.AreEqual(7, Game.Crew.riskSequence);
        }

        [Test] public void TiredOvertimeCanTriggerEvenWithSafeMachine()
        {
            Game.Crew.dayStamina = 20;
            Game.Crew.riskSequence = 3; // 0.158 exceeds 12%, falls below doubled risk at low stamina.
            Game.SetCrewPolicy(ShiftPolicy.Lembur);
            Receive(444); Assert.AreEqual(1, Game.Crew.incidents);
        }

        [Test] public void SmelterFineChargesCashAndCarriesShortfallUntilCashIsAvailable()
        {
            Game.InvestigateCrew(500, 3);
            Assert.Zero(Game.Cash);
            Assert.AreEqual(250000000, Game.Crew.lastFine);
            Assert.AreEqual(150000000, Game.Crew.unpaid);
            Game.Yard.stock[1] = 15000;
            double net = Game.CrewShipGrade(1, Business, 4000, 530, 0, 100, 3);
            Game.PayShipment(532); Game.CrewHour(533);
            Assert.AreEqual(150000000 - net, Game.Crew.unpaid);
            Assert.Zero(Game.Cash);
        }

        [Test] public void InvestigationStopsLoaderButResidentsCanStillHaveBlockedTruckTowed()
        {
            Game.CrewConfirmedAccident(500);
            Game.Yard.stuck = true;
            Assert.Throws<InvalidOperationException>(() => Game.CrewRescue(false, 501, 2500000, 100));
            double cash = Game.Cash;
            Game.CrewRescue(true, 501, 2500000, 100);
            Assert.IsFalse(Game.Yard.stuck); Assert.AreEqual(cash - 2500000, Game.Cash);
        }
    }
}
