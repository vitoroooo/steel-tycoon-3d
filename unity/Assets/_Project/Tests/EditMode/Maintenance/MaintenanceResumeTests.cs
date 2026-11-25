using System;
using NUnit.Framework;
using Steel.Core.Equipment;
using Steel.Core.State.Saving;
using UnityEngine;

namespace Steel.Tests.Maintenance
{
    public sealed class MaintenanceResumeTests
    {
        [Test]
        public void UnitySavePreservesPendingEmergencyDonorStatusAndHeavyLoadSequence()
        {
            var live = MaintenanceFixture.Yard();
            var loader = live.Yard.Machine(true, 0);
            loader.condition = 0;
            live.RepairMachine(true, RepairMethod.Emergency, 0);
            var scale = live.Yard.Machine(false, 0);
            scale.needsOriginal = true;
            scale.condition = 0;
            var resumed = MaintenanceFixture.Yard();
            var saved = JsonUtility.FromJson<CareerSave>(JsonUtility.ToJson(live.Snapshot(3)));
            resumed.Load(saved, 3, 0);
            Assert.AreEqual(live.Cash, resumed.Cash);
            Assert.IsTrue(resumed.Yard.Machine(false, .1).needsOriginal);
            Assert.Zero(resumed.Yard.Machine(true, .24).condition);
            int liveAccidents = 0, resumedAccidents = 0;
            live.MaintenanceAccident += _ => liveAccidents++;
            resumed.MaintenanceAccident += _ => resumedAccidents++;
            for (int h = 1; h <= 5; h++)
            {
                live.Yard.stuck = resumed.Yard.stuck = true;
                if (h < 5) { live.Rescue(false, h, 0); resumed.Rescue(false, h, 0); }
                else
                {
                    Assert.Throws<InvalidOperationException>(() => live.Rescue(false, h, 0));
                    Assert.Throws<InvalidOperationException>(() => resumed.Rescue(false, h, 0));
                }
                Assert.AreEqual(loader.randomState, resumed.Yard.Machine(true, h).randomState);
                Assert.AreEqual(loader.condition, resumed.Yard.Machine(true, h).condition);
            }
            Assert.IsTrue(resumed.Yard.stuck);
            Assert.AreEqual(1, resumedAccidents);
            Assert.AreEqual(liveAccidents, resumedAccidents);
            Assert.AreEqual(1, resumed.Yard.Machine(true, 6).breakdowns);
        }

        [Test]
        public void LegacyYardWithoutMachinesOpensWithHealthyTools()
        {
            var s = MaintenanceFixture.Yard();
            var old = JsonUtility.FromJson<CareerSave>(JsonUtility.ToJson(s.Snapshot(3)));
            old.yard.machines = null;
            s.Load(old, 3, 0);
            Assert.AreEqual(100, s.Yard.Machine(true, 0).condition);
            Assert.AreEqual(100, s.Yard.Machine(false, 0).condition);
        }
    }
}
