using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.Presentation.Input;
using Steel.Services;
using Steel.Presentation.Vehicles;
using Steel.Presentation.Player;

namespace Steel.PlayTests
{
    public sealed class PickupSmokeTests
    {
        private sealed class MoveSource : IMoveInput
        {
            public Vector2 Value;
            public Vector2 Move => Value;
        }
        [UnityTest] public IEnumerator EnterDriveExitPickup()
        {
            yield return SceneManager.LoadSceneAsync("City"); yield return null;
            var host = Object.FindObjectOfType<GameHost>(); var driver = Object.FindObjectOfType<PickupDriver>();
            var player = Object.FindObjectOfType<PlayerMover>(); var v = host.Game.Vehicle;
            host.Game.SetPosition(v.X, v.Z); driver.Toggle(); yield return null;
            Assert.IsTrue(host.Game.Vehicle.Occupied); Assert.IsFalse(player.GetComponent<CharacterController>().enabled);
            Vector3 before = driver.transform.position;
            var source = new MoveSource { Value = Vector2.up }; driver.MoveInput = source;
            float end = Time.fixedTime + 2;
            while (Time.fixedTime < end) yield return new WaitForFixedUpdate();
            source.Value = Vector2.zero; end = Time.fixedTime + 1;
            while (Time.fixedTime < end) yield return new WaitForFixedUpdate();
            var after = driver.transform.position;
            Assert.Greater(Vector2.Distance(new Vector2(before.x, before.z), new Vector2(after.x, after.z)), 2);
            Assert.AreEqual(driver.transform.position.x, host.Game.Vehicle.X, .2f);
            Assert.AreEqual(driver.transform.position.z, host.Game.Vehicle.Z, .2f);
            driver.Toggle(); yield return null;
            Assert.IsFalse(host.Game.Vehicle.Occupied); Assert.IsTrue(player.GetComponent<CharacterController>().enabled);
            Assert.IsTrue(player.Visual.gameObject.activeSelf);
        }
        [UnityTest] public IEnumerator CameraSphereDoesNotCrossWall()
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.position = new Vector3(150, 3, 146); wall.transform.localScale = new Vector3(100, 100, 1);
            Physics.SyncTransforms();
            Vector3 camera = CameraCollision.Resolve(new Vector3(150, 3, 150), new Vector3(0, 1, -8));
            Assert.Greater(camera.z, 146.8f); Object.Destroy(wall); yield return null;
        }
        [UnityTest] public IEnumerator FallingPickupRecoversAtItsConfiguredSpawn()
        {
            yield return SceneManager.LoadSceneAsync("City"); yield return null;
            var host = Object.FindObjectOfType<GameHost>(); var driver = Object.FindObjectOfType<PickupDriver>();
            var v = host.Game.Vehicle; host.Game.SetPosition(v.X, v.Z); driver.Toggle();
            driver.GetComponent<Rigidbody>().position = new Vector3(150, host.Layout.FallY - 3, 150);
            float end = Time.fixedTime + .12f;
            while (Time.fixedTime < end) yield return new WaitForFixedUpdate();
            Assert.AreEqual(v.X, driver.transform.position.x, .05f);
            Assert.AreEqual(v.Z, driver.transform.position.z, .05f);
            Assert.Greater(driver.transform.position.y, host.Layout.FallY);
            Assert.AreEqual(v.X, host.Game.Vehicle.X, .05f);
            Assert.AreEqual(0, host.Game.Vehicle.Speed); Assert.IsTrue(host.Game.Vehicle.Occupied);
        }
    }
}
