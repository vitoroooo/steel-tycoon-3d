using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.Presentation.World;
using Steel.Services;

namespace Steel.PlayTests.World
{
    /// <summary>Proyek Bongkaran: the excavator works its breaker without digging into the ground, and the site holds reuse finds.</summary>
    public sealed class DemolitionTests
    {
        [UnityTest]
        public IEnumerator BreakerHammersLiftsAndNeverDigsBelowItsRest()
        {
            yield return SceneManager.LoadSceneAsync("City");
            var arm = Object.FindObjectOfType<BreakerArm>();
            Assert.IsNotNull(arm, "the excavator's arm");
            var mesh = arm.GetComponentInChildren<Renderer>();
            yield return null;
            float restTip = mesh.bounds.min.y, highest = 0, lowestTip = restTip, biggestStep = 0, last = arm.Raised;
            Time.timeScale = 4;
            for (float until = Time.time + 24; Time.time < until;)
            {
                yield return null;
                highest = Mathf.Max(highest, arm.Raised);
                lowestTip = Mathf.Min(lowestTip, mesh.bounds.min.y);
                biggestStep = Mathf.Max(biggestStep, Mathf.Abs(arm.Raised - last) / Mathf.Max(Time.deltaTime, 1e-4f)); last = arm.Raised;
            }
            Time.timeScale = 1;
            Assert.Greater(highest, arm.LiftDegrees * .9f, "the arm never lifted clear");
            Assert.Greater(lowestTip, restTip - .05f, "the chisel dug into the ground");
            Assert.Less(biggestStep, 40, "the arm snapped instead of easing (degrees per second)");
        }

        [Test]
        public void TheSiteHoldsAReuseFindInsideItsFence()
        {
            var layout = new CityLayout(new ResourceConfigSource());
            Assert.IsTrue(layout.Piles.Any(p => p.Position.X < -9 && p.Position.Z < -20 && p.Position.Z > -71),
                "no scrap to collect inside Proyek Bongkaran");
        }
    }
}
