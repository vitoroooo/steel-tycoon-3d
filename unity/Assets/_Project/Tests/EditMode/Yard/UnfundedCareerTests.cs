using System;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Yard
{
    public sealed class UnfundedCareerTests
    {
        [Test] public void DefaultCareerCanReachBandarAndReceiveARealDeposit()
        {
            var source = new ResourceConfigSource(); var g = new LocalGameService(source);
            var city = new CityLayout(source); var p = city.Piles[0]; Assert.AreEqual(500000,g.View.Cash);
            void Home() => g.SetPosition(city.Home.X,city.Home.Z);
            void Sell(int bin)
            {
                g.Classify(null,bin,false); g.InspectBid(0); g.SellLocal();
            }
            void Acquire(string id)
            {
                var v = g.VehicleOf(id); g.SetPosition(v.X,v.Z);
                if (id == FleetCatalog.Triseda) g.ClaimDealerGrant();
                g.BuyVehicle(id); g.EnterVehicle(id); g.ReportVehicle(id,city.Home.X,city.Home.Z,0);
                g.ExitVehicle(city.Home.X,city.Home.Z); Home();
            }
            g.SetPosition(p.Position.X,p.Position.Z);g.Classify(p.Id,0,false); g.PickUp(p.Id); Home(); Sell(0);
            Acquire(FleetCatalog.Triseda);
            int trips = 0;
            while (g.View.Cash < 285000000 && trips < 1500)
            {
                if (!g.VehicleOf(FleetCatalog.Pikap).Owned && g.View.Cash > 85500000) Acquire(FleetCatalog.Pikap);
                string id = g.VehicleOf(FleetCatalog.Pikap).Owned ? FleetCatalog.Pikap : FleetCatalog.Triseda;
                int area = id == FleetCatalog.Pikap ? 2 : 1;
                var offer = g.Route(id,area,false); Assert.Greater(offer.Kg,0);
                g.BookRoute(offer); g.CatchUp(offer.Minutes*60);
                g.EnterVehicle(id); g.CollectRoute(); Sell(3); g.ExitVehicle(city.Home.X,city.Home.Z); trips++;
            }
            Assert.Less(trips,1500); Assert.GreaterOrEqual(g.YardInfo(0).Rep,30);
            g.ApplyPermit(true); g.CatchUp(14400); g.Lease(); g.OpenYard();
            double bid = Math.Ceiling(g.YardInfo(0).Floor*1.08);
            g.SetBid(0,bid); double cash = g.View.Cash; g.Advance(4*600);
            Assert.AreEqual(250,g.YardInfo(0).Stock); Assert.AreEqual(cash-250*bid,g.View.Cash,.001);
            TestContext.WriteLine("CAREER_FROM_START routes=" + trips + " hours=" + g.GameHours.ToString("0.0"));
        }
    }
}
