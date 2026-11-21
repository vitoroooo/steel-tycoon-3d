using System;
using NUnit.Framework;
using Steel.Services;
using Steel.Tests.Support;

namespace Steel.Tests.Routes
{
    public sealed class RouteTests
    {
        private LocalGameService g;
        private CityLayout map;
        [SetUp] public void Start()
        {
            var source = new ResourceConfigSource();
            map = new CityLayout(source); g = new LocalGameService(source); Home();
        }
        private void Home() => g.SetPosition(map.Home.X, map.Home.Z);
        private RouteView Quote(bool buy = false) => g.Route("cart", 0, buy);

        [Test] public void FirstCartQueuesAndCollectsInHandSizedLoads()
        {
            var quote = Quote();
            Assert.AreEqual(20, quote.Kg); Assert.AreEqual(30, quote.Minutes);
            g.BookRoute(quote); g.BookRoute(Quote());
            g.Advance(1799); Assert.AreEqual(0, Quote().DepotKg);
            g.Advance(1); Assert.AreEqual(20, Quote().DepotKg);
            g.Advance(1800); Assert.AreEqual(40, Quote().DepotKg);
            Assert.AreEqual(15, g.CollectRoute()); Assert.AreEqual(25, Quote().DepotKg);
            Assert.Throws<InvalidOperationException>(() => g.CollectRoute());
            Assert.AreEqual(25, Quote().DepotKg);
        }

        [Test] public void SupplierGoodsAreChargedOnceAndFactoryMarginIsNegative()
        {
            var quote = Quote(true); double before = g.View.Cash;
            g.BookRoute(quote); Assert.AreEqual(before - quote.Goods, g.View.Cash);
            g.CatchUp(1800); g.CollectRoute(); g.SortAtHome();
            g.SetPosition(map.Factory.X, map.Factory.Z);
            var sale = g.PreviewSale();
            Assert.Less(sale.Profit, 0);
            Assert.AreEqual(sale.Revenue - quote.Goods * 15 / quote.Kg, sale.Profit, .001);
        }

        [Test] public void QuoteCannotBeChangedAndUnownedOrOutOfRangeVehicleCannotDispatch()
        {
            var forged = Quote(true); forged.Goods = 0;
            Assert.Throws<InvalidOperationException>(() => g.BookRoute(forged));
            Assert.Throws<InvalidOperationException>(() => g.Route("cart", 1, false));
            var bike = g.Route(FleetCatalog.Triseda, 1, false);
            Assert.AreEqual(45, bike.Minutes); Assert.AreEqual(40000, bike.Travel);
            Assert.Throws<InvalidOperationException>(() => g.BookRoute(bike));
            Assert.AreEqual(500000, g.View.Cash);
        }

        [Test] public void DailyStockIsSharedAndRefillsWithoutGrantingFreeGoods()
        {
            g = new LocalGameService(Wealthy.Source()); Wealthy.Own(g, FleetCatalog.Pikap); Home();
            g.BookRoute(g.Route(FleetCatalog.Pikap, 0, true));
            Assert.Zero(Quote().Kg);
            Assert.Throws<InvalidOperationException>(() => g.BookRoute(Quote()));
            g.Advance(14400);
            Assert.AreEqual(20, Quote().Kg);
            Assert.AreEqual(650, Quote().DepotKg);
        }
        [Test] public void OrdersLockVehicleAndEightHourQueueIsEnforced()
        {
            for (int i = 0; i < 16; i++) g.BookRoute(Quote());
            Assert.Throws<InvalidOperationException>(() => g.BookRoute(Quote()));
            g.SetPosition(g.Cart.X, g.Cart.Z);
            Assert.Throws<InvalidOperationException>(() => g.GrabCart());
            Assert.AreEqual(28800, g.CatchUp(99999));
            g.GrabCart(); Assert.IsTrue(g.Cart.Held);
            Assert.AreEqual(320, Quote().DepotKg);
        }
    }
}
