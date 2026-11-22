using System;
using NUnit.Framework;

namespace Steel.Tests.Events
{
    /// <summary>"Lonjakan harga" and "Inspeksi lingkungan" (PRD appendix E).</summary>
    public sealed class MillEventTests : EventFixture
    {
        private static readonly double[] Rates = { 10000, 13000, 16000 };
        private static readonly (string, double)[] Inspected = { ("inspectChance", 1), ("flashChance", 1e-12) };

        [Test] public void IbnOpeningMoreThanEightPercentUpIsAPriceSpike()
        {
            OpenYard();
            Game.AdvanceEvents(0, 10000, Rates);
            Game.AdvanceEvents(144, 10700, Rates);
            Assert.IsFalse(Game.PriceSpike(144), "7% up is an ordinary day");
            Game.AdvanceEvents(288, 11700, Rates);
            Assert.IsTrue(Game.PriceSpike(288));
            Assert.IsTrue(Game.PriceSpike(431));
            Assert.AreEqual(11700d / 10700 - 1, Game.Events.spike, 1e-9);
            StringAssert.Contains("LONJAKAN HARGA", Game.Events.note);
            Game.AdvanceEvents(432, 11700, Rates);
            Assert.IsFalse(Game.PriceSpike(432));
        }

        [Test] public void AnInspectionFinesAMillWithoutAnActiveK3Certificate()
        {
            OpenMill(Inspected);
            double cash = Game.Cash;
            Game.AdvanceEvents(24, 10000, Rates);
            Assert.AreEqual(cash - 100000000, Game.Cash);
            Game.AdvanceEvents(30, 10000, Rates);
            Assert.AreEqual(cash - 100000000, Game.Cash, "Once a game day at most");
        }

        [Test] public void AMillWithCompletePermitsPasses()
        {
            OpenMill(Inspected);
            Game.Mill.safetyReady = 10;
            double cash = Game.Cash;
            Game.AdvanceEvents(24, 10000, Rates);
            Assert.AreEqual(cash, Game.Cash);
            StringAssert.Contains("lulus", Game.Events.note);
        }

        [Test] public void AMillWithoutTheEnclosedShredderIsInspectedTwiceAsOften()
        {
            int Fines(bool shredder)
            {
                OpenMill(("inspectChance", 0.2), ("flashChance", 1e-12), ("inspectFine", 1000));
                Game.Mill.shredder = shredder;
                Crew(("newcomerHours", 0), ("twoEventWindowHours", 1e-9));
                double cash = Game.Cash;
                for (int day = 1; day <= 400; day++) Game.AdvanceEvents(day * 24, 10000, Rates);
                return (int)Math.Round((cash - Game.Cash) / 1000);
            }
            int open = Fines(false), closed = Fines(true);
            Assert.That(closed, Is.InRange(55, 105));
            Assert.That(open, Is.InRange(130, 190));
        }
    }
}
