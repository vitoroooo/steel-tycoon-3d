using System;
using NUnit.Framework;

namespace Steel.Tests.Events
{
    /// <summary>"Tender kilat" (PRD appendix E, Kasta 3).</summary>
    public sealed class FlashTenderTests : EventFixture
    {
        private static readonly double[] Rates = { 10000, 13000, 16000 };
        private static readonly (string, double)[] Tendered = { ("inspectChance", 1e-12), ("flashChance", 1) };

        [Test] public void AFlashTenderPaysAPremiumWhenDeliveredWithinOneGameDay()
        {
            OpenMill(Tendered);
            Game.AdvanceEvents(24, 10000, Rates);
            var e = Game.Events;
            Assert.IsTrue(e.flash);
            Assert.AreEqual(0, e.flashProduct, "A mill with only a furnace is asked for billet");
            Assert.That(e.flashKg, Is.InRange(5000, 15000));
            Assert.AreEqual(10800, e.flashPrice);
            Assert.AreEqual(48, e.flashUntil);
            Assert.Throws<InvalidOperationException>(() => Game.DeliverFlashTender(25), "Not before it is taken");
            Game.TakeFlashTender(25);
            double cash = Game.Cash, kg = e.flashKg; int reputation = Game.Yard.reputation;
            Assert.AreEqual(kg * 10800, Game.DeliverFlashTender(26));
            Assert.AreEqual(cash + kg * 10800, Game.Cash, 1e-3);
            Assert.AreEqual(20000 - kg, Game.Mill.goods[0], 1e-6);
            Assert.AreEqual(reputation + 2, Game.Yard.reputation);
            Assert.IsFalse(e.flash);
        }

        [Test] public void ATakenFlashTenderLeftUnfinishedCostsReputationAndAnUntakenOneJustLapses()
        {
            OpenMill(Tendered);
            Game.AdvanceEvents(24, 10000, Rates);
            int reputation = Game.Yard.reputation;
            Game.AdvanceEvents(47, 10000, Rates);
            Assert.IsTrue(Game.Events.flash, "Open for the whole game day");
            Game.AdvanceEvents(48, 10000, Rates);
            Assert.AreEqual(reputation, Game.Yard.reputation, "Nobody took the first one");
            Assert.IsTrue(Game.Events.flash, "The next day's tender");
            Game.Mill.goods[0] = 1000;
            Game.TakeFlashTender(49);
            Game.DeliverFlashTender(50);
            Assert.Throws<InvalidOperationException>(() => Game.DeliverFlashTender(50), "The warehouse is empty");
            Game.AdvanceEvents(72, 10000, Rates);
            Assert.AreEqual(reputation - 5, Game.Yard.reputation);
            Assert.Throws<InvalidOperationException>(() => Game.DeliverFlashTender(73));
        }
    }
}
