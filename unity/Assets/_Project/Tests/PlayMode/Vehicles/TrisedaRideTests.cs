using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Steel.PlayTests.Support;
using Steel.Presentation.Characters;
using Steel.Presentation.Input;
using Steel.Presentation.Player;
using Steel.Presentation.UI;
using Steel.Presentation.Vehicles;
using Steel.Services;

namespace Steel.PlayTests.Vehicles
{
    public sealed class TrisedaRideTests
    {
        private const string Id = FleetCatalog.Triseda;
        private sealed class Throttle : IMoveInput { public Vector2 Move => new Vector2(.6f, 1); }

        [UnityTest]
        public IEnumerator BoughtTrisedaIsRiddenInTheOpenWithHandsOnTheBar()
        {
            yield return OwnedFleet.LoadWealthyCity();
            var host = Object.FindObjectOfType<GameHost>();
            var hud = Object.FindObjectOfType<CityHud>();
            var mover = Object.FindObjectOfType<PlayerMover>();
            MotorDriver driver = OwnedFleet.Driver(Id);
            var seat = driver.GetComponent<RiderSeat>();
            var wheels = driver.GetComponent<MotorWheels>();
            OwnedFleet.PlacePlayer(host, driver.transform.position.x - 1.3f, driver.transform.position.z);
            yield return null;
            Assert.AreEqual("BELI TRISEDA", hud.ActionText.text);
            double cash = host.Game.View.Cash;
            hud.Act();
            Assert.AreEqual(cash - 12000000, host.Game.View.Cash, hud.NoticeText.text);
            yield return null;
            Assert.AreEqual("NAIK TRISEDA", hud.ActionText.text);
            hud.Act();
            Vector3 start = driver.transform.position;
            driver.MoveInput = new Throttle();
            Quaternion wheelBefore = wheels.Rolling[0].localRotation;
            for (float end = Time.time + 1.5f; Time.time < end;) yield return null;
            Assert.AreEqual(300, host.Game.View.CapacityKg);
            Assert.Greater(Vector3.Distance(start, driver.transform.position), 2, "The triseda must move");
            Assert.Greater(Quaternion.Angle(wheelBefore, wheels.Rolling[0].localRotation), 1, "Wheels must roll");
            Assert.Greater(Quaternion.Angle(Quaternion.identity, wheels.Steered[0].localRotation), 5, "Bar must turn");
            Assert.IsTrue(mover.Visual.gameObject.activeInHierarchy, "The rider stays visible");
            var pose = mover.Visual.GetComponent<SeatedPose>();
            var arms = mover.Visual.GetComponent<ArmReach>();
            Vector3 hips = (pose.LeftLeg.Upper.position + pose.RightLeg.Upper.position) / 2;
            Assert.Less(Vector3.Distance(hips, seat.Hips.position), .03f, "Hips on the saddle");
            Assert.Less(Vector3.Distance(pose.LeftLeg.Tip.position, seat.FootLeft.position), .05f, "Left foot on its peg");
            Assert.Less(Vector3.Distance(arms.Left.Tip.position, seat.GripLeft.position), .05f, "Left hand on the grip");
            Assert.Less(Vector3.Distance(arms.Right.Tip.position, seat.GripRight.position), .05f, "Right hand on the grip");
            driver.MoveInput = Object.FindObjectOfType<SliceInput>();
            for (float end = Time.time + 2.5f; Time.time < end;) yield return null;
            hud.Act();
            yield return null;
            Assert.IsFalse(host.Game.VehicleOf(Id).Occupied, hud.NoticeText.text);
            Assert.AreEqual(mover.transform, mover.Visual.parent, "The avatar returns to the player");
            Assert.IsTrue(mover.GetComponent<CharacterController>().enabled);
        }
    }
}
