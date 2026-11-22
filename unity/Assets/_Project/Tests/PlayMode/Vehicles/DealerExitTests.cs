using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.PlayTests.Support;
using Steel.Presentation.UI;
using Steel.Presentation.Vehicles;
using Steel.Services;

namespace Steel.PlayTests.Vehicles
{
    public sealed class DealerExitTests
    {
        private static readonly Vector3 MainRoad = new Vector3(3.5f, 0, 3), Junction = new Vector3(3, 0, -10),
            CrossStreet = new Vector3(-22, 0, -12);
        private const float RoadTop = .06f, TimeLimit = 30;

        [UnityTest]
        public IEnumerator TrisedaBoughtFromOpeningCashDrivesFromDealerToBothStreets()
        {
            yield return SceneManager.LoadSceneAsync("City");
            yield return null;
            var host = Object.FindObjectOfType<GameHost>();
            Assert.AreEqual(500000, host.Game.View.Cash);
            yield return FirstSetoran.RideTriseda(host, Object.FindObjectOfType<CityHud>());
            Assert.IsTrue(host.Game.VehicleOf(FleetCatalog.Triseda).Occupied, "The grant must put the player on the bike");
            MotorDriver driver = OwnedFleet.Driver(FleetCatalog.Triseda);
            var pilot = new Autopilot(driver.transform, MainRoad, Junction, CrossStreet);
            driver.MoveInput = pilot;
            float worstGap = 0, onMainRoad = float.NaN;
            for (float end = Time.time + TimeLimit; Time.time < end && !pilot.Done;)
            {
                yield return new WaitForFixedUpdate();
                worstGap = Mathf.Max(worstGap, Mathf.Abs(GroundProbe.Gap(driver.transform.position)));
                if (pilot.Reached == 2 && float.IsNaN(onMainRoad)) onMainRoad = driver.transform.position.y;
            }
            Vector3 stopped = driver.transform.position;
            Assert.AreEqual(3, pilot.Reached, "Stuck at " + stopped + " behind " + GroundProbe.Ahead(driver.transform));
            Assert.AreEqual(RoadTop, onMainRoad, .03f, "Wheels must be on the asphalt at the junction");
            Assert.Less(worstGap, .12f, "The bike floated or sank while crossing the pavement");
            for (float end = Time.time + 1.5f; Time.time < end;) yield return null;
            Assert.Less(Mathf.Abs(GroundProbe.Gap(driver.transform.position)), .03f, "Parked wheels must touch the road");
        }
    }
}
