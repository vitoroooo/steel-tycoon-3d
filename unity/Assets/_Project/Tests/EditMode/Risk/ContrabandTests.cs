using System;
using NUnit.Framework;
using Steel.Core.Risk;

namespace Steel.Tests.Risk
{
    /// <summary>Contraband (PRD 6.7): 3-5x margin, +15 heat a load, -5 a game day, raid 10% + heat/2, fine 3x and -25 reputation.</summary>
    public sealed class ContrabandTests : RiskFixture
    {
        [Test]
        public void TheRaidChanceFollowsThePrdFormulaAndAProtectorHalvesIt()
        {
            RiskRules rules = Rules();
            Assert.AreEqual(.10, rules.RaidChance(0, false), 1e-9);
            Assert.AreEqual(.30, rules.RaidChance(40, false), 1e-9, "PRD: heat 40 means 30%");
            Assert.AreEqual(.15, rules.RaidChance(40, true), 1e-9);
        }

        [Test]
        public void ALoadThatSlipsThroughPaysItsMarginAndHeatsUpThenCoolsFiveADay()
        {
            Open(Rules(("offerChance", 1), ("raidBase", 1e-9), ("raidPerHeat", 1e-9)));
            ContrabandOffer offer = Offer(30);
            Assert.That(offer.Gain / (offer.Value * .15), Is.InRange(3d, 5d), "Three to five times a legal margin");
            Assert.AreEqual(offer.Value * 3, offer.Fine, 1e-6);
            double cash = Game.Cash;
            Assert.IsFalse(Game.AcceptContraband(offer, 30));
            Assert.AreEqual(cash + offer.Gain, Game.Cash, 1e-6); Assert.AreEqual(15, Game.Risk.heat);
            Assert.IsFalse(Game.Contraband(31, PerKg).HasValue, "One answer a day");
            Assert.Throws<InvalidOperationException>(() => Game.AcceptContraband(offer, 31));
            Game.AdvanceRisk(49); Assert.AreEqual(5, Game.Risk.heat, "Two game days later");
            Assert.IsTrue(Game.Contraband(49, PerKg).HasValue, "A new day, a new offer");
        }

        [Test]
        public void ARaidSeizesTheLoadFinesThreeTimesItsWorthAndCostsTwentyFiveReputation()
        {
            Open(Rules(("offerChance", 1), ("raidBase", 1)));
            ContrabandOffer offer = Offer(30); double cash = Game.Cash;
            Assert.IsTrue(Game.AcceptContraband(offer, 30));
            Assert.AreEqual(cash - offer.Fine, Game.Cash, 1e-6); Assert.AreEqual(35, Game.Yard.reputation);
            Assert.AreEqual(15, Game.Risk.heat); StringAssert.Contains("RAZIA", Game.Risk.note);
            Assert.AreEqual(0, Game.Risk.seizedUntil, "Only a Kasta 3 operation is sealed");
        }

        [Test]
        public void ReportingRaisesReputationRefusingChangesNothingAndBothNeedThePlayerAtHisYard()
        {
            Open(Rules(("offerChance", 1)));
            double cash = Game.Cash;
            Game.Position = new Steel.Core.State.Slice.WorldPoint(0, 0);
            Assert.Throws<InvalidOperationException>(() => Game.ReportContraband(Offer(30), 30));
            Game.Position = Post;
            Game.ReportContraband(Offer(30), 30); Assert.AreEqual(63, Game.Yard.reputation);
            Game.RefuseContraband(Offer(54), 54);
            Assert.AreEqual(63, Game.Yard.reputation); Assert.AreEqual(cash, Game.Cash); Assert.AreEqual(0, Game.Risk.heat);
        }

        [Test]
        public void AnExposedProtectorDoublesTheNextPenalty()
        {
            Open(Rules(("offerChance", 1), ("raidBase", 2), ("protectorExposed", 1)));
            double cash = Game.Cash;
            Game.HireProtector(30);
            Assert.AreEqual(cash - 50000000, Game.Cash); Assert.IsTrue(Game.Risk.exposed);
            Assert.Throws<InvalidOperationException>(() => Game.HireProtector(31), "Already paid for this month");
            ContrabandOffer offer = Offer(30); cash = Game.Cash;
            Assert.IsTrue(Game.AcceptContraband(offer, 30), "Half of a certain raid is still a certain raid");
            Assert.AreEqual(cash - offer.Fine * 2, Game.Cash, 1e-6); Assert.AreEqual(10, Game.Yard.reputation);
        }
    }
}
