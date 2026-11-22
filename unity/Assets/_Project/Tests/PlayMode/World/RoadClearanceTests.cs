using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Steel.PlayTests.World
{
    public sealed class RoadClearanceTests
    {
        /// <summary>Carriageways between the kerbs, from above the kerb height up to a pikap's roof.</summary>
        private static readonly Bounds[] Roads =
        {
            new Bounds(new Vector3(0, 1.1f, 5), new Vector3(13, 1.4f, 98)),
            new Bounds(new Vector3(0, 1.1f, -12), new Vector3(83, 1.4f, 9))
        };

        [UnityTest]
        public IEnumerator NothingSolidStandsOnTheCarriageways()
        {
            yield return SceneManager.LoadSceneAsync("City");
            yield return null;
            foreach (Bounds road in Roads)
            {
                string[] blockers = Physics.OverlapBox(road.center, road.extents, Quaternion.identity,
                        Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)
                    .Select(c => c.name + " " + c.bounds.center.ToString("F1")).Distinct().ToArray();
                Assert.IsEmpty(blockers, "Vehicles cannot pass: " + string.Join("; ", blockers));
            }
        }
    }
}
