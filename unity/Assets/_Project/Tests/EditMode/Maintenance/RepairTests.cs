using System;
using NUnit.Framework;
using Steel.Core.Equipment;

namespace Steel.Tests.Maintenance
{
    public sealed class RepairTests
    {
        [Test]
        public void WearUsesOperatingTimeAndStrictHeavyBoundary()
        {
            var yard = MaintenanceFixture.Yard();
            var tool = yard.Yard.Machine(true, 0);
            Assert.AreEqual(WorkResult.Completed, tool.Work(yard.Maintenance, 0, 2, .8));
            Assert.AreEqual(99, tool.condition);
            Assert.AreEqual(WorkResult.Completed, tool.Work(yard.Maintenance, 2, .5, .80001));
            Assert.AreEqual(98, tool.condition);
            Assert.AreEqual(2.5, yard.Yard.Machine(true, 300).operatingHours);
            Assert.AreEqual(98, tool.condition);
            Assert.AreEqual(WorkResult.WornOut, tool.Work(yard.Maintenance, 300, 50, 1));
            Assert.AreEqual(51.5, tool.operatingHours);
            Assert.AreEqual(WorkResult.Unavailable, tool.Work(yard.Maintenance, 351, 1, .2));
        }

        [TestCase(RepairMethod.Original, 12000000, 48)]
        [TestCase(RepairMethod.Emergency, 1500000, .25)]
        public void RepairDebitsOnceAndBecomesUsableAtDeadline(RepairMethod method, double cost, double delay)
        {
            var yard = MaintenanceFixture.Yard();
            yard.Yard.Machine(true, 1).condition = 0;
            double cash = yard.Cash;
            double expense = 0;
            yard.RepairPaid += price => expense += price;
            yard.RepairMachine(true, method, 1);
            Assert.AreEqual(cash - cost, yard.Cash);
            Assert.AreEqual(cost, expense);
            Assert.Throws<InvalidOperationException>(() => yard.RepairMachine(true, method, 1));
            Assert.Zero(yard.Yard.Machine(true, 1 + delay - .001).condition);
            var repaired = yard.Yard.Machine(true, 1 + delay);
            Assert.AreEqual(100, repaired.condition);
            Assert.AreEqual(method == RepairMethod.Emergency ? 1 + delay + 24 : 0, repaired.riskUntil);
            Assert.AreEqual(cash - cost, yard.Cash);
        }

        [Test]
        public void DonorIsSacrificedImmediatelyAndRequiresOriginalParts()
        {
            var yard = MaintenanceFixture.Yard();
            yard.Yard.Machine(true, 4).condition = 0;
            yard.RepairMachine(true, RepairMethod.Donor, 4);
            Assert.AreEqual(500000000, yard.Cash);
            Assert.Zero(yard.Yard.Machine(false, 4).condition);
            Assert.IsTrue(yard.Yard.Machine(false, 4).needsOriginal);
            Assert.Zero(yard.Yard.Machine(true, 4.99).condition);
            Assert.AreEqual(100, yard.Yard.Machine(true, 5).condition);
            Assert.Throws<InvalidOperationException>(() => yard.RepairMachine(false, RepairMethod.Donor, 5));
            Assert.Throws<InvalidOperationException>(() => yard.RepairMachine(false, RepairMethod.Emergency, 5));
            yard.RepairMachine(false, RepairMethod.Original, 5);
            Assert.AreEqual(498000000, yard.Cash);
            Assert.AreEqual(100, yard.Yard.Machine(false, 53).condition);
            Assert.IsFalse(yard.Yard.Machine(false, 53).needsOriginal);
        }

        [Test]
        public void RejectedRepairLeavesCashDonorAndDeadlineUntouched()
        {
            var yard = MaintenanceFixture.Yard(20);
            var scale = yard.Yard.Machine(false, 0);
            scale.condition = 0;
            Assert.Throws<InvalidOperationException>(() => yard.RepairMachine(false, RepairMethod.Original, 0));
            yard.Yard.rescued = true;
            yard.Yard.rescueHour = 1;
            Assert.Throws<InvalidOperationException>(() => yard.RepairMachine(false, RepairMethod.Donor, 0));
            Assert.Zero(scale.repairDue);
            Assert.AreEqual(100, yard.Yard.Machine(true, 0).condition);
            Assert.AreEqual(20, yard.Cash);
        }
    }
}
