using System;
using NUnit.Framework;

namespace Steel.Tests.Events
{
    /// <summary>"Setoran berkotoran" and "Pengepul AI mogok" (PRD appendix E, Kasta 2).</summary>
    public sealed class DirtyDepositTests : EventFixture
    {
        private void Dirty()
        {
            OpenYard(("dirtyChance", 1));
            Assert.IsTrue(Game.HoldDeposit(1, 1000, Floor, Capacity, 30), "A cheating collector takes the scale");
            Assert.IsTrue(Game.Events.DirtyWaiting);
            Assert.That(Game.Events.dirt, Is.InRange(0.08, 0.25));
        }

        [Test] public void AcceptingPaysOnlyForTheMetalAndStocksOnlyTheMetal()
        {
            Dirty();
            double cash = Game.Cash, metal = 1000 * (1 - Game.Events.dirt);
            Game.AcceptDirtyDeposit(Capacity, 31);
            Assert.AreEqual(metal, Game.Yard.stock[1], 1e-6);
            Assert.AreEqual(cash - metal * 4000, Game.Cash, 1e-3);
            Assert.IsFalse(Game.Events.DirtyWaiting);
            Assert.Throws<InvalidOperationException>(() => Game.AcceptDirtyDeposit(Capacity, 31));
        }

        [Test] public void RejectingCostsTheLoadButKeepsCollectorsHonestForThreeGameDays()
        {
            Dirty();
            double cash = Game.Cash;
            Game.RejectDirtyDeposit(31);
            Assert.AreEqual(0, Game.Yard.stock[1]);
            Assert.AreEqual(cash, Game.Cash);
            Assert.IsFalse(Game.HoldDeposit(1, 1000, Floor, Capacity, 31 + 71), "Still honest on the third day");
            Assert.IsTrue(Game.HoldDeposit(1, 1000, Floor, Capacity, 31 + 72), "Then the cheats are back");
        }

        [Test] public void TheScaleStaysTakenUntilTheOwnerAnswersOrTheCollectorIsPaidWithTheCut()
        {
            Dirty();
            double metal = 1000 * (1 - Game.Events.dirt);
            Assert.IsTrue(Game.HoldDeposit(2, 1000, Floor, Capacity, 35), "No other load is weighed meanwhile");
            Assert.AreEqual(0, Game.Yard.stock[2]);
            Assert.IsTrue(Game.HoldDeposit(2, 1000, Floor, Capacity, 36), "After six game hours it settles itself");
            Assert.AreEqual(metal, Game.Yard.stock[1], 1e-6);
            Assert.IsFalse(Game.Events.DirtyWaiting);
        }

        [Test] public void AYardThatCannotPayKeepsTheChoiceToReject()
        {
            Dirty();
            Game.Yard.bids[1] = 1e9;
            Assert.Throws<InvalidOperationException>(() => Game.AcceptDirtyDeposit(Capacity, 31));
            Assert.IsTrue(Game.Events.DirtyWaiting);
            Game.RejectDirtyDeposit(31);
        }

        [Test] public void ACollectorsStrikeIsTheSameDayForEveryYardAndHalvesTheLoads()
        {
            var rules = Events();
            int strikes = 0;
            for (long day = 0; day < 1000; day++)
            {
                Assert.AreEqual(rules.CollectorStrike(day), Events().CollectorStrike(day));
                Assert.AreEqual(rules.CollectorStrike(day) ? 0.5 : 1, rules.Supply(day));
                if (rules.CollectorStrike(day)) strikes++;
            }
            Assert.That(strikes, Is.InRange(60, 140), "About one game day in ten");
        }
    }
}
