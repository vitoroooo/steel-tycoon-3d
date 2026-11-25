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
    /// <summary>The ring road exists for the biggest rig in the game: a trailer must lap it without touching anything.</summary>
    public sealed class RingRoadTests
    {
        private const double Cash = 2000000000;
        private const float Lane = 5, GameSecondsLimit = 420, Speedup = 8;
        private const int Step = 6;   // a waypoint every twelve metres

        [UnityTest, Timeout(600000)]
        public IEnumerator TrailerLapsTheRingRoadInItsLane()
        {
            yield return OwnedFleet.LoadWealthyCity(Cash);
            var host = Object.FindObjectOfType<GameHost>();
            MotorDriver driver = OwnedFleet.Buy(host, FleetCatalog.Trailer);
            host.Game.EnterVehicle(FleetCatalog.Trailer);
            MapWay ring = MapRoute.Way(MapPlan.Load(), MapPlan.Ring);
            Vector3[] lap = MapRoute.Along(ring, Step, ring.xs.Length - 1, Step, Lane);
            Vector3 start = MapRoute.Lane(ring, 0, Lane);
            // On the asphalt, not at the height of the dealer floor; through the transform, so the hitch reads the new heading.
            driver.transform.SetPositionAndRotation(start - Vector3.up * GroundProbe.Gap(start), Quaternion.LookRotation(lap[0] - start));
            Physics.SyncTransforms();
            var pilot = new Autopilot(driver.transform, lap);
            driver.MoveInput = pilot;
            var hitch = driver.GetComponent<TrailerHitch>();
            hitch.enabled = false; hitch.enabled = true;   // the deck was left behind by the move: couple it straight again
            float worstGap = 0, widest = 0, scale = Time.timeScale;
            Vector3 worstAt = start, widestAt = start;
            try
            {
                Time.timeScale = Speedup;
                for (float end = Time.time + GameSecondsLimit; Time.time < end && !pilot.Done;)
                {
                    yield return new WaitForFixedUpdate();
                    float gap = Mathf.Abs(GroundProbe.Gap(driver.transform.position));
                    if (gap > worstGap) { worstGap = gap; worstAt = driver.transform.position; }
                    if (Mathf.Abs(hitch.Fold) > widest) { widest = Mathf.Abs(hitch.Fold); widestAt = driver.transform.position; }
                }
            }
            finally { Time.timeScale = scale; }
            Assert.IsTrue(pilot.Done, "Trailer stopped after " + pilot.Reached + " of " + lap.Length + " waypoints at "
                + driver.transform.position + " behind " + GroundProbe.Ahead(driver.transform));
            Assert.Less(worstGap, .05f, "The trailer floated or sank on the ring road at " + worstAt);
            Assert.Less(widest, hitch.MaxFold, "The ring road jackknifes a trailer at " + widestAt);
        }
    }
}
