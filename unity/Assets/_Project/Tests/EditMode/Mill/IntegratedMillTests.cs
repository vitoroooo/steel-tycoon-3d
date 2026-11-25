using NUnit.Framework;
using Steel.Core.State.Saving;
using Steel.Services;
using UnityEngine;

namespace Steel.Tests.Mill
{
    public sealed class IntegratedMillTests
    {
        private readonly IConfigSource src = new ResourceConfigSource();
        private CareerSave state;
        [SetUp] public void AtMill()
        {
            var f = new CityLayout(src).Factory;
            state = JsonUtility.FromJson<CareerSave>(new LocalGameService(src).Save(1000));
            state.x = f.X; state.z = f.Z;
            state.yard.bandar = state.yard.leased = state.yard.applied = true; state.yard.leaseUntil = 720;
            state.mill.opened = state.mill.filed = true; state.mill.plantValue = 2950000000;
            state.finance.level = 40;
        }
        private LocalGameService Load() => LocalGameService.Resume(src,JsonUtility.ToJson(state),1000,out _);
        [Test] public void BadDebtLiquidatesTheMillAndKeepsOneTierAndEarnedLevel()
        {
            state.finance.debt = 6000000000; state.finance.interestDue = 86400; state.cash = 0;
            var g = Load(); g.CatchUp(86400*2);
            Assert.AreEqual(2,g.View.Caste); Assert.IsFalse(g.MillInfo(0,1).Open);
            Assert.AreEqual(40,g.Finances().Level); Assert.Zero(g.Finances().Debt);
            Assert.AreEqual(75000000,g.View.Cash);
        }
        [Test] public void RfidReducesTheActualWaitBetweenInboundTransfers()
        {
            double Run(bool rfid)
            {
                state.mill.rfid = rfid; state.cash = 20000000;
                state.satellites = new[] { new SatelliteSave { district = 1, stock = 30000 } };
                var g = Load(); g.HaulSatellite(1);
                Assert.Throws<System.InvalidOperationException>(() => g.HaulSatellite(1));
                return JsonUtility.FromJson<CareerSave>(g.Save(2000)).mill.gateReady;
            }
            Assert.AreEqual(1d/6,Run(false),.000001); Assert.AreEqual(1d/20,Run(true),.000001);
        }
        [Test] public void HeatMovesDocumentedCostFromScrapToSteelAndTaxesOnlyTheSaleMargin()
        {
            state.cash = 50000000; state.yard.stock[1] = state.finance.knownKg[1] = 10000;
            state.finance.stockCost[1] = 50000000;
            var g = Load(); g.ChargeFurnace(0,1);
            g = LocalGameService.Resume(src,g.Save(2000),6800,out _);
            double receipts = g.SellSteel(0);
            Assert.AreEqual(receipts-69800000,g.Finances().Profit,.001);
            Assert.AreEqual(.15*System.Math.Max(0,receipts-69800000),g.Finances().TaxEstimate,.001);
        }
    }
}
