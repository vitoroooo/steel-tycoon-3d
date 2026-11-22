using System;
using NUnit.Framework;
using Steel.Core.State.Saving;
using Steel.Services;
using UnityEngine;

namespace Steel.Tests.Mill
{
    public sealed class MillTests
    {
        private readonly IConfigSource source = new ResourceConfigSource();
        private CareerSave state;
        private LocalGameService g;
        private CityLayout city;
        [SetUp] public void AtFactory()
        {
            city = new CityLayout(source);
            state = JsonUtility.FromJson<CareerSave>(new LocalGameService(source).Save(1000));
            state.cash = 20000000000; state.yard.applied = state.yard.leased = state.yard.bandar = true;
            state.finance.level = 40;
            state.yard.leaseUntil = 1000; state.yard.reputation = 80;
            state.yard.stock[1] = 40000;
            state.x = city.Factory.X; state.z = city.Factory.Z;
            Reload();
        }
        private void Reload() => g = LocalGameService.Resume(source,JsonUtility.ToJson(state),1000,out _);
        private void Open() { g.ApplyAmdal(true); g.Advance(72*600); g.BuildMill(); }
        [Test] public void AmdalNeedsCompletePapersThenTimeRepAndCapital()
        {
            double cash = g.View.Cash;
            Assert.Throws<InvalidOperationException>(() => g.ApplyAmdal(false)); Assert.AreEqual(cash,g.View.Cash);
            g.ApplyAmdal(true); Assert.Throws<InvalidOperationException>(() => g.BuildMill());
            g.Advance(72*600); state = JsonUtility.FromJson<CareerSave>(g.Save(1000));
            state.yard.reputation = 69; Reload(); Assert.Throws<InvalidOperationException>(() => g.BuildMill());
            state.yard.reputation = 70; state.cash = 2999999999; Reload();
            Assert.Throws<InvalidOperationException>(() => g.BuildMill());
            state.cash++; Reload(); g.BuildMill(); Assert.AreEqual(3,g.View.Caste);
            Assert.AreEqual(50000000,g.View.Cash);
        }
        [Test] public void ChargedFurnaceResumesOnlyOnceAndSellsFinishedProductsAtIbn()
        {
            Open(); Assert.Throws<InvalidOperationException>(() => g.ChargeFurnace(1,1));
            double cash = g.View.Cash; g.ChargeFurnace(0,1);
            Assert.AreEqual(30000,g.YardInfo(1).Stock); Assert.AreEqual(cash-19800000,g.View.Cash);
            Assert.Throws<InvalidOperationException>(() => g.ChargeFurnace(0,1));
            Assert.Throws<InvalidOperationException>(() => g.SellSteel(0));
            g = LocalGameService.Resume(source,g.Save(1000),5800,out _);
            Assert.AreEqual(9000,g.MillInfo(0,1).Goods);
            g = LocalGameService.Resume(source,g.Save(5800),5800,out _);
            Assert.AreEqual(9000*g.Prices.Ibn,g.SellSteel(0),.01);
            Assert.Throws<InvalidOperationException>(() => g.SellSteel(0));
            g.InstallLine(0); g.InstallLine(1); g.InstallLine(2);
            for (int product = 1; product <= 2; product++)
            {
                g.ChargeFurnace(product,1); g.Advance(8*600);
                Assert.AreEqual(9300*g.Prices.Ibn*(product == 1 ? 1.2 : 1.35),g.SellSteel(product),.01);
            }
        }
    }
}
