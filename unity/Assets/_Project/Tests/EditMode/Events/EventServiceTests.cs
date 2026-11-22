using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Events
{
    /// <summary>The events as the game service runs them hour by hour, across a save and a reload.</summary>
    public sealed class EventServiceTests
    {
        private static IConfigSource Source(double dirtyChance)
        {
            IConfigSource src = new OverrideConfigSource(new ResourceConfigSource(), "City", "startCash", 900000000);
            src = new OverrideConfigSource(src, "Business", "minimumReputation", 1);
            return new OverrideConfigSource(TestConfig.Calm(src), "Events", "dirtyChance", dirtyChance);
        }

        private static LocalGameService Yard(IConfigSource src)
        {
            var city = new CityLayout(src); var g = new LocalGameService(src);
            var p = city.Piles[0]; g.SetPosition(p.Position.X, p.Position.Z);
            g.Classify(p.Id, 0, false); g.PickUp(p.Id);
            g.SetPosition(city.Home.X, city.Home.Z); g.Classify(null, 0, false); g.InspectBid(0); g.SellLocal();
            g.ApplyPermit(true); g.Lease(); g.Advance(24 * 600); g.OpenYard();
            for (int grade = 0; grade < 4; grade++) g.SetBid(grade, g.YardInfo(grade).Floor * 1.1);
            return g;
        }

        [Test] public void ADirtyLoadWaitsOnTheScaleAcrossAReloadAndIsPaidOnce()
        {
            IConfigSource src = Source(1);
            LocalGameService g = Yard(src);
            g.Advance(600);
            EventView before = g.Events;
            Assert.IsTrue(before.Dirty);
            Assert.That(before.Dirt, Is.InRange(0.08, 0.25));
            g = LocalGameService.Resume(src, g.Save(1), 1, out _);
            EventView after = g.Events;
            Assert.IsTrue(after.Dirty);
            Assert.AreEqual(before.Dirt, after.Dirt, 1e-12); Assert.AreEqual(before.DirtyKg, after.DirtyKg, 1e-9);
            double cash = g.View.Cash;
            StringAssert.Contains("potongan", g.AnswerDeposit(true));
            Assert.AreEqual(cash - after.DirtyPrice, g.View.Cash, 1e-3);
            g = LocalGameService.Resume(src, g.Save(2), 2, out _);
            Assert.IsFalse(g.Events.Dirty);
            Assert.AreEqual(cash - after.DirtyPrice, g.View.Cash, 1e-3);
        }

        [Test] public void WithHonestCollectorsTheYardFillsAsBefore()
        {
            LocalGameService g = Yard(Source(1e-12));
            for (int hour = 0; hour < 8; hour++) g.Advance(600);
            Assert.IsFalse(g.Events.Dirty);
            double stock = 0;
            for (int grade = 0; grade < 4; grade++) stock += g.YardInfo(grade).Stock;
            Assert.Greater(stock, 0);
        }
    }
}
