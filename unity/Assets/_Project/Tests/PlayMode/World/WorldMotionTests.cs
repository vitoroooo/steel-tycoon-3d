using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.Presentation.World;

namespace Steel.PlayTests.World
{
    /// <summary>The things that should move in the city do, and stay within believable limits while doing it.</summary>
    public sealed class WorldMotionTests
    {
        [UnityTest]
        public IEnumerator WashingAndLeavesSwayGentlyAroundTheirRest()
        {
            yield return SceneManager.LoadSceneAsync("City");
            var sways = Object.FindObjectsOfType<WindSway>();
            Assert.Greater(sways.Count(s => s.name.StartsWith("GoyangKain")), 20, "washing lines");
            Assert.Greater(sways.Count(s => s.name.StartsWith("GoyangDaun")), 10, "banana crowns");
            foreach (var sway in sways) sway.enabled = true;   // the test camera sees none of them
            var rest = sways.ToDictionary(s => s, s => s.transform.localRotation);
            float largest = 0, moved = 0;
            for (float until = Time.time + 2; Time.time < until;)
            {
                yield return null;
                foreach (var sway in sways)
                {
                    float off = Quaternion.Angle(rest[sway], sway.transform.localRotation);
                    largest = Mathf.Max(largest, off); moved += off;
                }
            }
            Assert.Greater(moved, 0, "nothing swayed");
            Assert.Less(largest, 25, "washing should swing, not spin");
        }

        [UnityTest]
        public IEnumerator GantryTrolleyTravelsSmoothlyAndStaysOnItsBridge()
        {
            yield return SceneManager.LoadSceneAsync("City");
            var crane = Object.FindObjectOfType<CraneTravel>();
            Assert.IsNotNull(crane, "the smelter's gantry crane");
            float start = crane.Trolley.localPosition.x, last = start, fastest = 0;
            Quaternion hanging = crane.Load.localRotation;
            Time.timeScale = 8;
            for (float until = Time.time + 40; Time.time < until;)
            {
                yield return null;
                float x = crane.Trolley.localPosition.x;
                fastest = Mathf.Max(fastest, Mathf.Abs(x - last) / Mathf.Max(Time.deltaTime, 1e-4f)); last = x;
                Assert.LessOrEqual(Mathf.Abs(x), crane.Reach + .2f, "trolley left the bridge");
                Assert.Less(Quaternion.Angle(hanging, crane.Load.localRotation), 30, "load swung wildly");
            }
            Time.timeScale = 1;
            Assert.Greater(Mathf.Abs(last - start), 1, "trolley never travelled");
            Assert.LessOrEqual(fastest, crane.Speed * 1.05f, "trolley jumped instead of easing");
        }
    }
}
