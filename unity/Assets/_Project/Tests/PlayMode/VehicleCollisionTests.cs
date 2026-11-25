using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.Services;
using Steel.PlayTests.Support;
using Steel.Presentation.Input;
using Steel.Presentation.Vehicles;

namespace Steel.PlayTests
{
    public sealed class VehicleCollisionTests
    {
        private sealed class Steering : IMoveInput { public Vector2 Move => Vector2.one; }
        [UnityTest] public IEnumerator HardSteeringBesideWallNeverPenetratesOrCrossesIt()
        {
            yield return OwnedFleet.LoadWealthyCity();
            var host = Object.FindObjectOfType<GameHost>(); var driver = OwnedFleet.Buy(host, FleetCatalog.Pikap);
            var body = driver.GetComponent<Rigidbody>(); var box = driver.GetComponent<BoxCollider>();
            body.position = new Vector3(0, .035f, 30); body.rotation = Quaternion.identity;
            host.Game.ReportVehicle(FleetCatalog.Pikap, 0, 30, 0);
            float end = Time.fixedTime + .12f;
            while (Time.fixedTime < end) yield return new WaitForFixedUpdate();
            host.Game.SetPosition(0, 30); driver.Toggle(); driver.MoveInput = new Steering();
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.position = new Vector3(1.025f, 1.5f, 30);
            wall.transform.localScale = new Vector3(.1f, 3, 60); Physics.SyncTransforms();
            var obstacle = wall.GetComponent<BoxCollider>(); end = Time.fixedTime + 2;
            while (Time.fixedTime < end)
            {
                yield return new WaitForFixedUpdate();
                bool overlap = Physics.ComputePenetration(box, body.position, body.rotation,
                    obstacle, wall.transform.position, wall.transform.rotation, out _, out float depth);
                Assert.IsFalse(overlap && depth > .001f, "Steering penetrated the wall by " + depth);
                Assert.Less(body.position.x, .975f);
            }
            Assert.Greater(body.position.z, 30.02f); Object.Destroy(wall); yield return null;
        }
    }
}
