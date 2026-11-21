using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.Services;
using Steel.Presentation.Player;
using Steel.Presentation.UI;
using Steel.Presentation.Vehicles;

namespace Steel.PlayTests.Vehicles
{
    public sealed class CartPushTests
    {
        private const float PushSeconds = 1.2f;

        [UnityTest]
        public IEnumerator GrabbedCartRollsAheadOfThePlayerAndParksWhereReleased()
        {
            yield return SceneManager.LoadSceneAsync("City");
            yield return null;
            var host = Object.FindObjectOfType<GameHost>();
            var hud = Object.FindObjectOfType<CityHud>();
            var cart = Object.FindObjectOfType<CartPusher>();
            var mover = Object.FindObjectOfType<PlayerMover>();
            mover.enabled = false;   // the test walks the player itself; there is no keyboard in batch mode
            mover.GetComponent<CharacterController>().enabled = false;
            Vector3 grips = cart.transform.position - cart.transform.forward * cart.Reach;
            Place(host, mover, grips);
            yield return null;
            Assert.AreEqual("DORONG GEROBAK", hud.ActionText.text);
            hud.Act();
            Assert.IsTrue(host.Game.Cart.Held, hud.NoticeText.text);
            Assert.AreEqual(50, host.Game.View.CapacityKg);
            Quaternion wheelBefore = cart.WheelLeft.localRotation;
            Vector3 heading = mover.Visual.forward;
            for (float end = Time.time + PushSeconds; Time.time < end;)
            {
                Place(host, mover, mover.transform.position + heading * host.Game.Cart.PushSpeed * Time.deltaTime);
                yield return null;
            }
            Vector3 lead = cart.transform.position - mover.transform.position;
            lead.y = 0;
            Assert.AreEqual(cart.Reach, lead.magnitude, .05f, "Cart is not at arm's length");
            Assert.Greater(Vector3.Dot(lead.normalized, heading), .95f, "Cart is not in front of the player");
            Assert.Greater(Quaternion.Angle(wheelBefore, cart.WheelLeft.localRotation), 5, "Wheels did not roll");
            Assert.IsFalse(cart.GetComponent<Collider>().enabled, "A held cart would shove its own pusher");
            hud.Act();
            Assert.IsFalse(host.Game.Cart.Held);
            Vector3 parked = cart.transform.position;
            Place(host, mover, mover.transform.position - heading * 6);
            yield return null;
            yield return null;
            Assert.Less(Vector3.Distance(parked, cart.transform.position), .01f, "Parked cart moved");
            Assert.IsTrue(cart.GetComponent<Collider>().enabled);
        }

        private static void Place(GameHost host, PlayerMover mover, Vector3 point)
        {
            mover.transform.position = point;
            host.Game.SetPosition(point.x, point.z);
        }
    }
}
