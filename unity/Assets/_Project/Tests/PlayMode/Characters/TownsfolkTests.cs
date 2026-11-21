using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.Presentation.Characters.Npc;
using Steel.Presentation.Player;

namespace Steel.PlayTests.Characters
{
    /// <summary>Bystanders populate the city, walk their routes without sinking or leaving, and hold their poses.</summary>
    public sealed class TownsfolkTests
    {
        [UnityTest]
        public IEnumerator WalkersCoverGroundAndStayOnTheirFeet()
        {
            yield return SceneManager.LoadSceneAsync("City");
            yield return null;
            var walkers = Object.FindObjectsOfType<Stroller>();
            Assert.GreaterOrEqual(walkers.Length, 3, "walkers");
            Assert.GreaterOrEqual(Object.FindObjectsOfType<Bystander>().Length, 6, "people standing, sitting and squatting");
            Object.FindObjectOfType<PlayerMover>().transform.position = new Vector3(0, .3f, 62);   // out of everyone's way
            var start = walkers.ToDictionary(w => w, w => w.transform.position);
            var farthest = walkers.ToDictionary(w => w, w => 0f);
            Time.timeScale = 6;
            for (float until = Time.time + 40; Time.time < until;)
            {
                yield return null;
                foreach (var walker in walkers)
                {
                    Vector3 at = walker.transform.position;
                    farthest[walker] = Mathf.Max(farthest[walker], Vector3.Distance(start[walker], at));
                    Assert.That(at.y, Is.InRange(-.3f, .6f), walker.name + " left the ground");
                    Assert.Less(Mathf.Abs(walker.transform.eulerAngles.x) + Mathf.Abs(walker.transform.eulerAngles.z), .01f,
                        walker.name + " tipped over");
                }
            }
            Time.timeScale = 1;
            foreach (var walker in walkers)
                Assert.Greater(farthest[walker], 4, walker.name + " hardly moved: something blocks the route");
        }
    }
}
