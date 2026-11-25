using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.Services;
using Steel.Presentation.Player;
using Steel.Presentation.World;

namespace Steel.PlayTests
{
    public sealed class WorldSafetySmokeTests
    {
        [UnityTest] public IEnumerator FallRecoversAtConfiguredSpawn()
        {
            yield return SceneManager.LoadSceneAsync("City"); yield return null;
            var host = Object.FindObjectOfType<GameHost>(); var player = Object.FindObjectOfType<PlayerMover>();
            var controller = player.GetComponent<CharacterController>(); double cash = host.Game.View.Cash;
            Assert.IsNotNull(player.GetComponent<FallRecovery>());
            controller.enabled = false; player.transform.position = new Vector3(20, host.Layout.FallY - 2, 20);
            controller.enabled = true; yield return null; yield return null;
            Assert.AreEqual(host.Layout.Spawn.X, player.transform.position.x, .05f);
            Assert.AreEqual(host.Layout.Spawn.Z, player.transform.position.z, .05f);
            Assert.Greater(player.transform.position.y, host.Layout.FallY);
            Assert.AreEqual(cash, host.Game.View.Cash);
        }
        [UnityTest] public IEnumerator SouthBoundaryStopsWalkingOffGround()
        {
            yield return SceneManager.LoadSceneAsync("City"); yield return null;
            var host = Object.FindObjectOfType<GameHost>(); var player = Object.FindObjectOfType<PlayerMover>();
            var controller = player.GetComponent<CharacterController>();
            foreach (string side in new[] { "selatan", "utara", "barat", "timur" })
                Assert.IsNotNull(GameObject.Find("Batas " + side).GetComponent<BoxCollider>());
            controller.enabled = false;
            player.transform.position = new Vector3(0, host.Layout.SpawnY, host.Layout.BoundaryMinZ + 2);
            controller.enabled = true; Physics.SyncTransforms(); controller.Move(Vector3.back * 10);
            Assert.Greater(player.transform.position.z, host.Layout.BoundaryMinZ);
        }
    }
}
