using System;
using NUnit.Framework;
using Steel.Core.Risk;

namespace Steel.Tests.Risk
{
    /// <summary>A gang at the gate (PRD 6.3): pay 2% of the month's sales, talk it to 1%, or hold the gate; unanswered, it settles itself.</summary>
    public sealed class ExtortionTests : RiskFixture
    {
        private static readonly (string, double)[] Certain = { ("extortChance", 1), ("extortCap", 1), ("deterMost", 1e-9) };

        private void Arrive(double turnover = 500000000)
        {
            Open(Rules(Certain));
            Game.CreditProceeds(turnover, 0);
            Game.AdvanceRisk(25);
            Assert.IsTrue(Game.Risk.demand, "The gang comes on the day's roll");
        }

        [Test]
        public void TheGangAsksTwoPercentOfTheMonthsSalesAndPayingBuysAQuietMonth()
        {
            Arrive();
            Assert.AreEqual(10000000, Game.Risk.demandDue, 1e-6); Assert.AreEqual(9, Game.Risk.thugs, "Four, plus two for each of his 2.5 billion");
            double cash = Game.Cash; Game.PayGang(26);
            Assert.AreEqual(cash - 10000000, Game.Cash, 1e-6); Assert.IsFalse(Game.Risk.demand);
            for (int hour = 27; hour < 26 + 720; hour++) Game.AdvanceRisk(hour);
            Assert.IsFalse(Game.Risk.demand, "Nobody comes for a game month");
            Game.AdvanceRisk(26 + 720 + 24); Assert.IsTrue(Game.Risk.demand, "Then they are back");
        }

        [Test]
        public void TalkingWithAPerfectReputationHalvesItAndWithNoneFailsOnce()
        {
            Arrive(); Game.Yard.reputation = 100; double cash = Game.Cash;
            Assert.IsTrue(Game.HaggleGang(26)); Assert.AreEqual(cash - 5000000, Game.Cash, 1e-6); Assert.IsFalse(Game.Risk.demand);
            Arrive(); Game.Yard.reputation = 0; cash = Game.Cash;
            Assert.IsFalse(Game.HaggleGang(26)); Assert.IsTrue(Game.Risk.demand); Assert.AreEqual(cash, Game.Cash);
            Assert.Throws<InvalidOperationException>(() => Game.HaggleGang(26), "They only talk once");
        }

        [Test]
        public void GuardsPostedBeforehandHoldTheGateAndCannotBeChangedOnceTheGangIsThere()
        {
            Open(Rules(Certain));
            double cash = Game.Cash;
            for (int lane = 0; lane < RiskRules.Lanes; lane++)
            { Game.PostGuard(lane * RiskRules.Columns + 4, 1); Game.PostGuard(lane * RiskRules.Columns + 3, 2); }
            Assert.AreEqual(cash - 4 * (6000000 + 4000000), Game.Cash, 1e-6); Assert.AreEqual(8, Game.Risk.GuardCount);
            Game.AdvanceRisk(25); Assert.IsTrue(Game.Risk.demand);
            Assert.Throws<InvalidOperationException>(() => Game.PostGuard(0, 3));
            cash = Game.Cash;
            BattleResult result = Game.DefendGate(26);
            Assert.IsTrue(result.Held); Assert.AreEqual(cash, Game.Cash, "A held gate costs nothing");
            Assert.AreEqual(26 + 240, Game.Risk.quietUntil);
        }

        [Test]
        public void AnUnguardedGateFallsAndCostsHalfAgainTheDemand()
        {
            Arrive(); double cash = Game.Cash;
            Assert.IsFalse(Game.DefendGate(26).Held);
            Assert.AreEqual(cash - 15000000, Game.Cash, 1e-6); StringAssert.Contains("jebol", Game.Risk.note);
        }

        [Test]
        public void LeftUnansweredTheLastSecuritySettingAnswersAfterTwelveHours()
        {
            Arrive(); double cash = Game.Cash;
            Game.AdvanceRisk(36); Assert.IsTrue(Game.Risk.demand, "Still waiting an hour before the deadline");
            Game.AdvanceRisk(37); Assert.IsFalse(Game.Risk.demand);
            Assert.AreEqual(cash - 10000000, Game.Cash, 1e-6, "Paid on its own");
            Arrive(); Game.SetAutoDefend(true); cash = Game.Cash;
            Game.AdvanceRisk(37);
            Assert.AreEqual(cash - 15000000, Game.Cash, 1e-6, "Told to hold an empty gate, it falls on its own");
        }

        [Test]
        public void APlayerWithoutAYardIsLeftAloneAndGuardsAndWealthMoveTheOdds()
        {
            Open(Rules(Certain)); Game.Yard.bandar = false;
            Game.AdvanceRisk(25); Assert.IsFalse(Game.Risk.demand);
            Assert.Less(Rules().ExtortChance(1e9, 16), Rules().ExtortChance(1e9, 0), "Guards on the grid keep gangs away");
            Assert.Greater(Rules().ExtortChance(5e9, 0), Rules().ExtortChance(1e9, 0), "Wealth draws them");
        }
    }
}
