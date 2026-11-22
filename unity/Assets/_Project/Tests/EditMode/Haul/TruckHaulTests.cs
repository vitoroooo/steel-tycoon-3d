using System;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Haul
{
    /// <summary>Hauling yard stock in the player's own truck: the loader fills the bed over game time and the mill pays after weighing.</summary>
    public sealed class TruckHaulTests
    {
        private IConfigSource src;
        private LocalGameService g;
        private CityLayout city;

        [SetUp] public void Start()
        {
            src = new OverrideConfigSource(new ResourceConfigSource(), "City", "startCash", 2000000000);
            src = new OverrideConfigSource(src, "Business", "minimumReputation", 1);
            city = new CityLayout(src); g = new LocalGameService(src);
            var p = city.Piles[0]; g.SetPosition(p.Position.X, p.Position.Z);
            g.Classify(p.Id, 0, false); g.PickUp(p.Id);
            g.SetPosition(city.Home.X, city.Home.Z); g.Classify(null, 0, false); g.InspectBid(0); g.SellLocal();
            g.ApplyPermit(true); g.Lease(); g.Advance(24 * 600); g.OpenYard();
            g.SetBid(0, 6000); g.Advance(40 * 600);   // the yard buys 250 kg of HMS 1 each fourth hour
        }

        /// <summary>Buys the truck at the dealer and stands it, engine off, at a post.</summary>
        private void Park(string truck, SliceLocation post)
        {
            var v = g.VehicleOf(truck);
            if (!v.Owned) { g.SetPosition(v.X, v.Z); g.BuyVehicle(truck); }
            g.SetPosition(g.VehicleOf(truck).X, g.VehicleOf(truck).Z); g.EnterVehicle(truck);
            g.ReportVehicle(truck, post.X + 6, post.Z, 0); g.ExitVehicle(post.X + 4, post.Z);
            g.SetPosition(post.X, post.Z);
        }

        [Test]
        public void TheLoaderFillsTheBedAtTenTonsAnHourAndStopsWhenTheTruckLeaves()
        {
            double stock = g.YardInfo(0).Stock; Assert.GreaterOrEqual(stock, 2000);
            g.SetPosition(g.YardPost.X, g.YardPost.Z);
            Assert.AreEqual("yardpost", g.Nearby.Id);
            Assert.Throws<InvalidOperationException>(() => g.LoadTruck(0), "No truck stands at the yard yet");
            Park(FleetCatalog.Engkel, g.YardPost);
            g.LoadTruck(0);
            Assert.IsTrue(g.YardDock.Loading); Assert.Zero(g.YardDock.LoadKg);
            g.Advance(60);   // a tenth of a game hour: one ton
            Assert.AreEqual(1000, g.YardDock.LoadKg, 1e-6); Assert.AreEqual(stock - 1000, g.YardInfo(0).Stock, 1e-6);
            Assert.AreEqual(1000.0 / 8000, g.BedShare(FleetCatalog.Engkel), 1e-9);
            g.SetPosition(g.VehicleOf(FleetCatalog.Engkel).X, g.VehicleOf(FleetCatalog.Engkel).Z);
            g.EnterVehicle(FleetCatalog.Engkel); g.ReportVehicle(FleetCatalog.Engkel, 0, 0, 0); g.Advance(60);
            Assert.AreEqual(1000, g.BedShare(FleetCatalog.Engkel) * 8000, 1e-6, "Nothing is loaded onto a truck that drove off");
            Assert.AreEqual(stock - 1000, g.YardInfo(0).Stock, 1e-6);
        }

        [Test]
        public void TheMillWeighsUnloadsAndPaysOnceLessTheRunningCost()
        {
            Park(FleetCatalog.Engkel, g.YardPost); g.LoadTruck(0);
            double stock = g.YardInfo(0).Stock; g.Advance(600);
            Assert.IsFalse(g.YardDock.Loading, "The heap ran out before the bed was full");
            Assert.AreEqual(stock, g.YardDock.LoadKg, 1e-6); Assert.Zero(g.YardInfo(0).Stock);
            g.SetPosition(g.GatePost.X, g.GatePost.Z);
            Assert.AreEqual("gate", g.Nearby.Id);
            Assert.Throws<InvalidOperationException>(() => g.WeighIn(), "The truck is still at the yard");
            Park(FleetCatalog.Engkel, g.GatePost);
            HaulView bill = g.GateDock; double cash = g.View.Cash;
            Assert.AreEqual(bill.LoadKg * bill.PerKg - 900000, bill.Net, 1, "Rust-free stock pays full weight less the engkel's run");
            Assert.AreEqual(bill.Net, g.WeighIn()); Assert.AreEqual(cash, g.View.Cash, "Paid after unloading, not before");
            Assert.Throws<InvalidOperationException>(() => g.WeighIn());
            g = LocalGameService.Resume(src, g.Save(1), 1, out _);
            Assert.IsTrue(g.GateDock.Unloading, "A reload keeps the truck under the crane");
            g.Advance(600);
            Assert.AreEqual(cash + bill.Net, g.View.Cash, 1e-6); Assert.Zero(g.BedShare(FleetCatalog.Engkel));
            g.Advance(600); Assert.AreEqual(cash + bill.Net, g.View.Cash, 1e-6);
        }
    }
}
