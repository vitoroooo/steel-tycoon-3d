using System;
using NUnit.Framework;

namespace Steel.Tests.Events
{
    /// <summary>"Temuan barang reuse" at a Kasta 2 yard (PRD 5.2 and appendix E).</summary>
    public sealed class ReuseFindTests : EventFixture
    {
        private const double Reference = 5000;

        private void Find(double depositKg = 5000)
        {
            OpenYard(("reuseChance", 1), ("reuseOrderChance", 1));
            Game.Yard.stock[1] = depositKg;
            Game.SortDeposit(1, depositKg, 30);
            Assert.IsTrue(Game.Events.ReuseWaiting);
        }

        [Test] public void AFindIsAtMostTwoTonnesAndComesOutOfTheSortedDeposit()
        {
            Find();
            Assert.That(Game.Events.reuseKg, Is.InRange(200, 2000));
            Assert.AreEqual(5000 - Game.Events.reuseKg, Game.Yard.stock[1], 1e-6);
            Find(150);
            Assert.AreEqual(150, Game.Events.reuseKg, "Never more than the deposit held");
            double kg = Game.Events.reuseKg;
            Game.SortDeposit(1, 5000, 31);
            Assert.AreEqual(kg, Game.Events.reuseKg, "One find at a time");
        }

        [Test] public void TokoKonstruksiPaysBetween160And250PercentOfTheReference()
        {
            Find();
            double cash = Game.Cash, kg = Game.Events.reuseKg;
            double paid = Game.SellReuseFind(Reference);
            Assert.That(paid / (kg * Reference), Is.InRange(1.6, 2.5));
            Assert.AreEqual(cash + paid, Game.Cash, 1e-3);
            Assert.Throws<InvalidOperationException>(() => Game.SellReuseFind(Reference), "Sold once");
        }

        [Test] public void KeptGoodsWaitForASpecialSpkThatPaysTheTopOfTheBand()
        {
            Find();
            double kg = Game.Events.reuseKg, cash = Game.Cash;
            Game.KeepReuseFind();
            Assert.AreEqual(kg, Game.Events.reuseStore);
            Assert.Throws<InvalidOperationException>(() => Game.DeliverReuseOrder(31), "No order yet");
            Game.ReuseHour(48, Reference);
            Assert.IsTrue(Game.Events.reuseOrder);
            Assert.AreEqual(kg * Reference * 2.5, Game.DeliverReuseOrder(50), 1e-3);
            Assert.AreEqual(cash + kg * Reference * 2.5, Game.Cash, 1e-3);
            Assert.AreEqual(0, Game.Events.reuseStore);
            Assert.Throws<InvalidOperationException>(() => Game.DeliverReuseOrder(51));
        }

        [Test] public void ASpecialSpkLapsesAfterOneGameDayAndKeptGoodsCanStillGoToTheShop()
        {
            Find();
            double kg = Game.Events.reuseKg;
            Game.KeepReuseFind();
            Game.ReuseHour(48, Reference);
            Assert.Throws<InvalidOperationException>(() => Game.DeliverReuseOrder(72), "A game day later it is gone");
            Assert.AreEqual(kg * Reference * 1.6, Game.SellReuseStore(Reference), 1e-3);
            Assert.Throws<InvalidOperationException>(() => Game.SellReuseStore(Reference));
        }

        [Test] public void AnUnansweredFindSellsItselfAfterOneGameDay()
        {
            Find();
            double cash = Game.Cash;
            Game.ReuseHour(53, Reference);
            Assert.IsTrue(Game.Events.ReuseWaiting);
            Game.ReuseHour(54, Reference);
            Assert.IsFalse(Game.Events.ReuseWaiting);
            Assert.Greater(Game.Cash, cash);
        }
    }
}
