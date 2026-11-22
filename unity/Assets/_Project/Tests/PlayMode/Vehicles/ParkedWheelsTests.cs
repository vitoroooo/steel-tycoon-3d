using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.PlayTests.Support;

namespace Steel.PlayTests.Vehicles
{
    /// <summary>Every tyre in a freshly loaded city rests on the surface under it: none sunk in, none floating.</summary>
    public sealed class ParkedWheelsTests
    {
        private const float Tolerance = .03f;
        private static readonly string[] Parked = { "Gerobak dorong", "Motor triseda", "Pikap bekas" };

        [UnityTest]
        public IEnumerator TyresRestOnTheGround()
        {
            yield return SceneManager.LoadSceneAsync("City");
            yield return null;
            AssertTyresRest("as built");   // nothing may sink or hover before the physics settle it
            yield return new WaitForSeconds(1);
            AssertTyresRest("after settling");
        }

        private static void AssertTyresRest(string moment)
        {
            foreach (string name in Parked)
            {
                Transform vehicle = GameObject.Find(name).transform;
                Transform[] wheels = vehicle.GetComponentsInChildren<Transform>()
                    .Where(part => part.name.StartsWith("Wheel_")).ToArray();
                Assert.IsNotEmpty(wheels, name + " has no wheels");
                foreach (Transform wheel in wheels)
                {
                    Bounds tyre = wheel.GetComponentsInChildren<Renderer>().Select(part => part.bounds)
                        .Aggregate((all, next) => { all.Encapsulate(next); return all; });
                    var contact = new Vector3(tyre.center.x, tyre.min.y, tyre.center.z);
                    Assert.Less(Mathf.Abs(GroundProbe.Gap(contact)), Tolerance, name + " / " + wheel.name + " " + moment);
                }
            }
        }
    }
}
