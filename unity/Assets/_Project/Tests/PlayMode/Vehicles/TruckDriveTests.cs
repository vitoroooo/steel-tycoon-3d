using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Steel.PlayTests.Support;
using Steel.Presentation.Vehicles;
using Steel.Presentation.Vehicles.Towing;
using Steel.Services;

namespace Steel.PlayTests.Vehicles
{
    /// <summary>The heavy trucks leave the pool under their own power, and a trailer deck follows its tractor.</summary>
    public sealed class TruckDriveTests
    {
        private const double Cash = 2000000000;
        private const float TimeLimit = 40;

        [UnityTest]
        public IEnumerator EveryTruckDrivesDownTheMainRoadOnItsWheels()
        {
            foreach (string id in FleetCatalog.Heavy)
            {
                yield return OwnedFleet.LoadWealthyCity(Cash);
                var host = Object.FindObjectOfType<GameHost>();
                MotorDriver driver = OwnedFleet.Buy(host, id);
                host.Game.EnterVehicle(id);
                Vector3 start = driver.transform.position;
                var pilot = new Autopilot(driver.transform, new Vector3(start.x, 0, start.z - 25));
                driver.MoveInput = pilot;
                float worstGap = 0;
                for (float end = Time.time + TimeLimit; Time.time < end && !pilot.Done;)
                {
                    yield return new WaitForFixedUpdate();
                    worstGap = Mathf.Max(worstGap, Mathf.Abs(GroundProbe.Gap(driver.transform.position)));
                }
                Assert.IsTrue(pilot.Done, id + " stuck at " + driver.transform.position + " behind " + GroundProbe.Ahead(driver.transform));
                Assert.Less(worstGap, .05f, id + " floated or sank on the road");
            }
        }

        [UnityTest]
        public IEnumerator TrailerDeckSwingsInATurnAndStraightensBehindTheTractor()
        {
            yield return OwnedFleet.LoadWealthyCity(Cash);
            var host = Object.FindObjectOfType<GameHost>();
            MotorDriver driver = OwnedFleet.Buy(host, FleetCatalog.Trailer);
            host.Game.EnterVehicle(FleetCatalog.Trailer);
            var hitch = driver.GetComponent<TrailerHitch>();
            Vector3 start = driver.transform.position;
            var pilot = new Autopilot(driver.transform, new Vector3(-3, 0, start.z - 14), new Vector3(-3, 0, start.z - 50));
            driver.MoveInput = pilot;
            float widest = 0;
            for (float end = Time.time + TimeLimit; Time.time < end && !pilot.Done;)
            {
                yield return new WaitForFixedUpdate();
                widest = Mathf.Max(widest, Mathf.Abs(hitch.Fold));
            }
            Assert.IsTrue(pilot.Done, "Trailer stuck at " + driver.transform.position + " behind " + GroundProbe.Ahead(driver.transform));
            Assert.Greater(widest, 4, "The deck must lag behind the tractor while it changes lanes");
            Assert.Less(widest, hitch.MaxFold, "A lane change must not jackknife the trailer");
            Assert.Less(Mathf.Abs(hitch.Fold), 3, "After a straight run the deck is back in line");
        }
    }
}
