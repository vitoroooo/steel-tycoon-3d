using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Steel.PlayTests.Support;
using Steel.Presentation.Vehicles;
using Steel.Presentation.Vehicles.Towing;
using Steel.Services;
using Steel.Services.Map;

namespace Steel.PlayTests.Vehicles
{
    /// <summary>The heavy trucks leave the dealer's forecourt under their own power and turn onto its street.</summary>
    public sealed class TruckDriveTests
    {
        private const double Cash = 2000000000;
        private const float TimeLimit = 60, Lane = 2.5f, Kerb = .3f;
        private const int Pieces = 25;   // about fifty metres up the street

        [UnityTest]
        public IEnumerator EveryTruckLeavesTheDealerAndDrivesUpItsStreet()
        {
            foreach (string id in FleetCatalog.Heavy)
            {
                yield return OwnedFleet.LoadWealthyCity(Cash);
                MotorDriver driver = Board(id, out Autopilot pilot);
                float worstGap = 0, widest = 0;
                var hitch = driver.GetComponent<TrailerHitch>();
                for (float end = Time.time + TimeLimit; Time.time < end && !pilot.Done;)
                {
                    yield return new WaitForFixedUpdate();
                    worstGap = Mathf.Max(worstGap, Mathf.Abs(GroundProbe.Gap(driver.transform.position)));
                    if (hitch != null) widest = Mathf.Max(widest, Mathf.Abs(hitch.Fold));
                }
                Assert.IsTrue(pilot.Done, id + " stuck at " + driver.transform.position + " behind " + GroundProbe.Ahead(driver.transform));
                Assert.Less(worstGap, Kerb, id + " left the ground by more than a kerb between the forecourt and the street");
                Assert.Less(Mathf.Abs(GroundProbe.Gap(driver.transform.position)), .05f, id + " does not rest on the street");
                if (hitch == null) continue;
                Assert.Greater(widest, 4, "The deck must lag behind the tractor as it turns out of the dealer");
                Assert.Less(widest, hitch.MaxFold, "Turning onto the street must not jackknife the trailer");
            }
        }

        /// <summary>Buys the truck, sits in it and plots the way out: straight off the forecourt, then left-lane up the street.</summary>
        private static MotorDriver Board(string id, out Autopilot pilot)
        {
            var host = Object.FindObjectOfType<GameHost>();
            MotorDriver driver = OwnedFleet.Buy(host, id);
            host.Game.EnterVehicle(id);
            MapPlan plan = MapPlan.Load();
            Vector3 start = driver.transform.position;
            (MapWay street, int index) = MapRoute.Nearest(plan, new Vector2(start.x, start.z));
            int from = Mathf.Min(index + 6, street.xs.Length - 1), to = Mathf.Min(index + Pieces, street.xs.Length - 1);
            pilot = new Autopilot(driver.transform, MapRoute.Along(street, from, to, 4, Lane));
            driver.MoveInput = pilot;
            return driver;
        }
    }
}
