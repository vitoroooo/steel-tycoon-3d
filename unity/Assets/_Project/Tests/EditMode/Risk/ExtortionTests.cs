using System;
using NUnit.Framework;
using Steel.Core.Risk;

namespace Steel.Tests.Risk
{
    /// <summary>A gang at the gate (PRD 6.3): pay 2%, talk it to 1%, or hold the gate (appendix D.1); unanswered, it settles itself.</summary>
    public sealed class ExtortionTests : RiskFixture
    {
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
        public void LeftUnansweredTheLastSecuritySettingAnswersAfterTwelveHours()
        {
            Arrive(); double cash = Game.Cash;
            Game.AdvanceRisk(36); Assert.IsTrue(Game.Risk.demand, "Still waiting an hour before the deadline");
            Game.AdvanceRisk(37); Assert.IsFalse(Game.Risk.demand);
            Assert.AreEqual(cash - 10000000, Game.Cash, 1e-6, "Paid on its own");
            Arrive(); Game.SetAutoDefend(true); cash = Game.Cash;
            Game.AdvanceRisk(37);
            Assert.AreEqual(cash - 60000000, Game.Cash, 1e-6, "Told to hold an empty gate, it falls on its own");
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
