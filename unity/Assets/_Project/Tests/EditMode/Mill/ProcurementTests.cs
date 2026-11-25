using System;
using NUnit.Framework;
using Steel.Core.State.Saving;
using Steel.Services;
using UnityEngine;

namespace Steel.Tests.Mill
{
    public sealed class ProcurementTests
    {
        private LocalGameService g;
        private readonly IConfigSource src = new ResourceConfigSource();
        [SetUp] public void Factory()
        {
            var s = JsonUtility.FromJson<CareerSave>(new LocalGameService(src).Save(1000));
            s.cash = 2000000000; s.mill.opened = s.mill.filed = s.mill.rolling = true;
            s.finance.level = 40;
            s.mill.safetyReady = 1;
            s.mill.goods[1] = 320000;
            s.yard.applied = s.yard.leased = s.yard.bandar = true; s.yard.leaseUntil = 1000; s.yard.reputation = 100;
            var f = new CityLayout(src).Factory; s.x = f.X; s.z = f.Z;
            g = LocalGameService.Resume(src,JsonUtility.ToJson(s),1000,out _);
        }
        [Test] public void ClosedBidAwardsOnceAndLocksTheActualHBeamQuantity()
        {
            g.Advance(600); var start = g.GovernmentOrder(0); g.OfferBridgeSteel(0);
            Assert.Throws<InvalidOperationException>(() => g.OfferBridgeSteel(1));
            Assert.Throws<InvalidOperationException>(() => g.DeliverBridgeSteel());
            g = LocalGameService.Resume(src,g.Save(1000),1000+600*24,out _);
            Assert.IsTrue(g.GovernmentOrder(0).Awarded);
            Assert.AreEqual(300000*start.UnitPrice,g.DeliverBridgeSteel());
            Assert.AreEqual(20000,g.MillInfo(1,0).Goods);
            Assert.Throws<InvalidOperationException>(() => g.DeliverBridgeSteel());
            g = LocalGameService.Resume(src,g.Save(20000),20000,out _);
            Assert.AreEqual(300000,g.GovernmentOrder(0).Delivered);
        }
        [Test] public void MissingTheDeadlineClosesTheAwardWithoutMintingPayment()
        {
            g.Advance(600); double cash = g.View.Cash, payroll = g.CrewInfo().Wage;
            g.OfferBridgeSteel(0); g.Advance(240*600);
            Assert.IsFalse(g.GovernmentOrder(0).Awarded);
            Assert.AreEqual(95,g.YardInfo(0).Rep); Assert.AreEqual(cash-10*payroll,g.View.Cash);
        }
        [Test] public void TwoDistrictStoresPayForTheirOwnStockAndHaulOnlyOnce()
        {
            g.LeaseSatellite(1); g.LeaseSatellite(2);
            Assert.Throws<InvalidOperationException>(() => g.LeaseSatellite(1));
            Assert.Throws<InvalidOperationException>(() => g.LeaseSatellite(0));
            g.PriceSatellite(1,true); double cash = g.View.Cash;
            g.CatchUp(600); Assert.AreEqual(250,g.Satellite(1).Stock); Assert.Zero(g.Satellite(2).Stock);
            Assert.AreEqual(cash-250*g.Satellite(1).Bid,g.View.Cash);
            for (int i = 0; i < 65 && g.Satellite(1).Stock < 15000; i++)
            { g.PriceSatellite(1,true); g.Advance(4*600); }
            g = LocalGameService.Resume(src,g.Save(10000),10000,out _);
            double before = g.Satellite(1).Stock; g.HaulSatellite(1);
            Assert.AreEqual(before-15000,g.Satellite(1).Stock); Assert.AreEqual(15000,g.YardInfo(1).Stock);
            Assert.Throws<InvalidOperationException>(() => g.HaulSatellite(1));
        }
    }
}
