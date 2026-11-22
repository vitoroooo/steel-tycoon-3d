using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Slice
{
    public sealed class DailySupplyTests
    {
        private ResourceConfigSource source;
        private LocalGameService g;
        private CityLayout map;
        [SetUp] public void Start()
        {
            source = new ResourceConfigSource(); g = new LocalGameService(source); map = new CityLayout(source);
        }
        private void At(int site) => g.SetPosition(map.Piles[site].Position.X, map.Piles[site].Position.Z);
        private void Take(int site) { At(site); g.Inspect("p" + site); g.PickUp("p" + site); }

        [Test]
        public void OnlyEmptySitesRefillOnTheHourAndCarriedItemIsUntouched()
        {
            Take(0);
            At(1); g.Inspect("p1");
            g.Advance(599);
            Assert.IsTrue(g.IsCollected("p0"));
            g.Advance(1);
            Assert.IsFalse(g.IsCollected("p0"));
            Assert.IsTrue(g.Nearby.Inspected, "Untouched stock is not replaced");
            At(0);
            Assert.IsFalse(g.Nearby.Inspected, "New stock needs its own inspection");
            g.SetPosition(map.Home.X, map.Home.Z); g.SortAtHome();
            g.SetPosition(map.Factory.X, map.Factory.Z);
            Assert.AreEqual(12, g.PreviewSale().Kilograms);
            g.ConfirmSale();
            g.CatchUp(8 * 3600);
            Take(0);
            Assert.AreEqual(12, g.View.CarriedKg, "Offline days do not stack multiple supplies");
        }

        [Test]
        public void SitesRefillOneAtATimeInTheOrderTheyWereEmptiedAlsoAfterResume()
        {
            g.SetPosition(g.Cart.X, g.Cart.Z); g.GrabCart();
            Take(3); Take(0); Take(5);
            Assert.AreEqual(7, g.Income.StreetSites); Assert.AreEqual(10, g.Income.RestockMinutes, .001);
            g.ReadStreetTip();
            g.Advance(300); Assert.AreEqual(5, g.Income.RestockMinutes, .001);
            g = LocalGameService.Resume(source, g.Save(5000), 5000, out _);
            Assert.IsTrue(g.StreetTipRead); Assert.AreEqual(7, g.Income.StreetSites);
            g.Advance(300);
            Assert.IsFalse(g.IsCollected("p3")); Assert.IsTrue(g.IsCollected("p0")); Assert.IsTrue(g.IsCollected("p5"));
            g.Advance(600);
            Assert.IsFalse(g.IsCollected("p0")); Assert.IsTrue(g.IsCollected("p5"));
            g.Advance(600);
            Assert.AreEqual(10, g.Income.StreetSites); Assert.Zero(g.Income.RestockMinutes);
        }
    }
}
