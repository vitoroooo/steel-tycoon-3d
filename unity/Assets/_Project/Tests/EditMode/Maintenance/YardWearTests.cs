using System;
using NUnit.Framework;
using Steel.Core.Equipment;

namespace Steel.Tests.Maintenance
{
    public sealed class YardWearTests
    {
        [Test]
        public void OnlyReceivableLotsWearTheScaleAndBrokenScaleStopsDeposits()
        {
            var s = MaintenanceFixture.Yard();
            s.SetBid(1, 100, 1);
            s.Receive(1, 110, 250, 60000, 1);
            s.Receive(1, 90, 250, 200, 1);
            var scale = s.Yard.Machine(false, 1);
            Assert.Zero(scale.operatingHours);
            s.Receive(1, 90, 2400, 60000, 2);
            s.Receive(1, 90, 2401, 60000, 3);
            Assert.AreEqual(100 - .125 - .5, scale.condition);
            Assert.AreEqual(.5, scale.operatingHours);
            scale.condition = .05;
            s.Receive(1, 90, 250, 60000, 4);
            s.Receive(1, 90, 250, 60000, 5);
            Assert.Zero(scale.condition);
            Assert.AreEqual(4801, s.Yard.stock[1]);
            Assert.AreEqual(500000000 - 4801 * 100, s.Cash);
        }

        [Test]
        public void LoaderWearUsesWetHandlingTimeAndRefusesUnfinishedShipment()
        {
            var s = MaintenanceFixture.Yard();
            var config = MaintenanceFixture.Business;
            var loader = s.Yard.Machine(true, 0);
            s.Yard.stock[1] = 30000;
            double net = s.ShipGrade(1, config, 5000, 1, 1);
            Assert.AreEqual(1.95, loader.operatingHours, 1e-9);
            Assert.AreEqual(100 - .975, loader.condition, 1e-9);
            s.PayShipment(3);
            double cash = s.Cash;
            Assert.AreEqual(500000000 + net, cash);
            loader.condition = .25;
            Assert.Throws<InvalidOperationException>(() => s.ShipGrade(1, config, 5000, 3, 0));
            Assert.Zero(loader.condition);
            Assert.Throws<InvalidOperationException>(() => s.ShipGrade(1, config, 5000, 4, 0));
            Assert.AreEqual(15000, s.Yard.stock[1]);
            Assert.Zero(s.Yard.payout);
            Assert.AreEqual(cash, s.Cash);
        }

        [Test]
        public void BrokenLoaderKeepsTrailerStuckButPaidTowWorks()
        {
            var s = MaintenanceFixture.Yard();
            s.Yard.stuck = true;
            s.Yard.Machine(true, 0).condition = 0;
            Assert.Throws<InvalidOperationException>(() => s.Rescue(false, 0, 2500000));
            Assert.IsTrue(s.Yard.stuck);
            s.Rescue(true, 0, 2500000);
            Assert.IsFalse(s.Yard.stuck);
            Assert.AreEqual(497500000, s.Cash);
            Assert.Zero(s.Yard.Machine(true, 0).operatingHours);
        }

        [Test]
        public void UnsafeOfflineDepositDoesNotAdvanceRiskOrSpendCash()
        {
            var s = MaintenanceFixture.Yard();
            var scale = s.Yard.Machine(false, 0);
            scale.riskUntil = 24;
            int cursor = scale.randomState;
            s.SetBid(1, 100, 0);
            s.MaintenanceOffline = () => true;
            s.Receive(1, 90, 2500, 60000, 1);
            Assert.AreEqual(cursor, scale.randomState);
            Assert.Zero(s.Yard.stock[1]);
            Assert.AreEqual(500000000, s.Cash);
            s.Receive(1, 90, 2500, 60000, 24);
            Assert.AreEqual(2500, s.Yard.stock[1]);
        }

        [Test]
        public void EmergencyRollsOnlyForHeavyAttemptsAndStopsAtWindowEnd()
        {
            var s = MaintenanceFixture.Yard();
            var loader = s.Yard.Machine(true, 0);
            loader.riskUntil = 24;
            int initial = loader.randomState;
            loader.Work(s.Maintenance, 1, 1, .8);
            Assert.AreEqual(initial, loader.randomState);
            loader.Work(s.Maintenance, 2, 1, .81);
            int rolled = loader.randomState;
            Assert.AreNotEqual(initial, rolled);
            loader.Work(s.Maintenance, 24, 1, 1);
            Assert.AreEqual(rolled, loader.randomState);
            Assert.Zero(loader.riskUntil);
        }
    }
}
