using System.Collections;
using NUnit.Framework;
using Steel.Presentation.Characters.Animals;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Steel.PlayTests.Characters
{
    public sealed class ChickenTests
    {
        [UnityTest]
        public IEnumerator BirdsPeckAndWalkAroundTheKampung()
        {
            yield return SceneManager.LoadSceneAsync("City");
            yield return null;
            var birds = Object.FindObjectsOfType<YardChicken>();
            Assert.AreEqual(4, birds.Length);
            var bird = System.Array.Find(birds, b => b.name == "Jago kampung");
            Vector3 start = bird.transform.position;
            var orbit = Camera.main.GetComponent<Steel.Presentation.Player.OrbitCamera>();
            orbit.enabled = false;
            Camera.main.transform.position = start + Vector3.up * 2;
            float far = 0, pitch = 0;
            float scale = Time.timeScale;
            try
            {
                Time.timeScale = 4;
                for (float until = Time.time + 14; Time.time < until;)
                {
                    yield return null;
                    far = Mathf.Max(far, Vector3.Distance(start, bird.transform.position));
                    pitch = Mathf.Max(pitch, Quaternion.Angle(Quaternion.identity, bird.Pose.Body.localRotation));
                    Assert.That(bird.transform.position.y, Is.InRange(-.2f, .4f));
                }
                Assert.Greater(far, 1);
                Assert.Greater(pitch, 25);
            }
            finally { Time.timeScale = scale; orbit.enabled = true; }
        }
    }
}
