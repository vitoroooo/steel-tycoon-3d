using System;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Yard
{
    public sealed class YardTests
    {
        private IConfigSource src;
        private LocalGameService g;
        private CityLayout city;
        [SetUp] public void Start()
        {
            src = new OverrideConfigSource(new ResourceConfigSource(),"City","startCash",900000000);
            src = new OverrideConfigSource(src,"Business","minimumReputation",1);
            src = TestConfig.Calm(src);
            city = new CityLayout(src); g = new LocalGameService(src);
            g.SetPosition(city.Home.X,city.Home.Z);
        }
        private void Rep()
        {
            var p = city.Piles[0]; g.SetPosition(p.Position.X,p.Position.Z);
            g.Classify(p.Id,0,false); g.PickUp(p.Id);
            g.SetPosition(city.Home.X,city.Home.Z); g.Classify(null,0,false); g.InspectBid(0); g.SellLocal();
        }
        [Test] public void PermitRentReputationAndCapitalGateTheYard()
        {
            g.Lease(); g.ApplyPermit(false); Assert.AreEqual(48,g.YardInfo(0).PermitHours);
            Assert.Throws<InvalidOperationException>(() => g.ApplyPermit(true));
            g.Advance(48*600); Assert.Throws<InvalidOperationException>(() => g.OpenYard());
            Rep(); double before = g.View.Cash;
            g.OpenYard(); Assert.AreEqual(before-175000000,g.View.Cash); Assert.AreEqual(2,g.View.Caste);
            Assert.Throws<InvalidOperationException>(() => g.OpenYard());
            g.BuyScanner(true); var p = city.Piles[1]; g.SetPosition(p.Position.X,p.Position.Z);
            StringAssert.Contains("HMS 2",g.Classify(p.Id,-1,true)); Assert.IsTrue(g.Nearby.Inspected);
        }
        [Test] public void CompetitiveBidPaysForDepositsOnceAndStopsWhenLeaseEnds()
        {
            Rep(); g.ApplyPermit(true); g.Lease(); g.Advance(24*600); g.OpenYard();
            g.SetBid(0,1); g.Advance(4*600); Assert.Zero(g.YardInfo(0).Stock);
            g.SetBid(0,6000); double before = g.View.Cash;
            g = LocalGameService.Resume(src,g.Save(1000),3400,out var elapsed);
            Assert.AreEqual(2400,elapsed); Assert.AreEqual(250,g.YardInfo(0).Stock);
            Assert.AreEqual(before-250*6000,g.View.Cash); Assert.Throws<InvalidOperationException>(() => g.ShipGrade(0));
            g.Advance(4*600*59); Assert.AreEqual(15000,g.YardInfo(0).Stock);
            double net = g.YardInfo(0).NetShipment; Assert.AreEqual(net,g.ShipGrade(0),.001);
            Assert.Zero(g.YardInfo(0).Stock); g.Advance(720*600); double stopped = g.YardInfo(0).Stock;
            Assert.IsFalse(g.YardInfo(0).Leased); g.Advance(4*600); Assert.AreEqual(stopped,g.YardInfo(0).Stock);
        }
    }
}
