using System;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Market
{
    public sealed class LocalBuyerTests
    {
        private LocalGameService g;
        private CityLayout city;
        [SetUp] public void Start()
        {
            var config = new ResourceConfigSource(); g = new LocalGameService(config); city = new CityLayout(config);
            g.SetPosition(g.Cart.X, g.Cart.Z); g.GrabCart();
        }
        private void Load(int i)
        {
            var p = city.Piles[i]; g.SetPosition(p.Position.X, p.Position.Z);
            g.Inspect(p.Id); g.PickUp(p.Id);
        }
        private void Home() => g.SetPosition(city.Home.X, city.Home.Z);
        [Test] public void ReuseAndSteelReachDifferentBuyersAndCashIsNetOfFreight()
        {
            Load(0); Load(8); Home(); g.SortAtHome(); double cash = g.View.Cash;
            var a = g.InspectBid(0);
            Assert.Greater(a.Freight, 0); Assert.AreEqual(a.Gross - a.Freight, g.SellLocal());
            Assert.AreEqual(24, g.View.CarriedKg);
            g.SetPosition(city.Factory.X, city.Factory.Z);
            Assert.Throws<InvalidOperationException>(() => g.PreviewSale());
            g.SetPosition(10, 40.5f); Assert.AreEqual("buyer", g.Nearby.Id);
            var b = g.InspectBid(2); Assert.Zero(b.Freight);
            Assert.GreaterOrEqual(b.Gross, b.Kg * 1.6 * g.Prices.ScrapReference - .001);
            g.SellLocal(); Assert.AreEqual(cash + a.Net + b.Net, g.View.Cash, .001);
            Assert.Zero(g.View.CarriedKg); Assert.Throws<InvalidOperationException>(() => g.SellLocal());
        }
        [Test] public void PricesDependOnBuyerAndGradeAndQuoteMustBeRechecked()
        {
            Load(2); Home(); g.SortAtHome();
            double pasar = g.InspectBid(0).Gross;
            Assert.Greater(g.InspectBid(1).Gross, pasar);
            g.Advance(600); Assert.Throws<InvalidOperationException>(() => g.SellLocal());
            var delivery = g.InspectBid(1); g.SetPosition(47,-8);
            Assert.Throws<InvalidOperationException>(() => g.SellLocal());
            Assert.Greater(g.InspectBid(1).Net, delivery.Net);
            g.SetPosition(-25,-20); Assert.Throws<InvalidOperationException>(() => g.SellLocal());
            Assert.AreEqual(25, g.View.CarriedKg);
        }
    }
}
