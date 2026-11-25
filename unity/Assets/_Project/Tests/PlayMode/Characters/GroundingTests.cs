using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.PlayTests.Support;
using Steel.Presentation.Characters;
using Steel.Presentation.Player;
using Steel.Services;

namespace Steel.PlayTests.Characters
{
    public sealed class GroundingTests
    {
        private const float Tolerance = .03f;

        /// <summary>Asphalt, pavement, the edge of a kerb, the dealer's yellow apron, and bare ground.</summary>
        private static readonly (string place, float x, float z)[] Spots =
        {
            ("jalan utama", 2, 20), ("trotoar", 9, 20), ("tepi kanstin", 7.08f, 20),
            ("jalan keluar dealer", 9, 3), ("tanah lapak", -16, 6)
        };

        [UnityTest]
        public IEnumerator PlayerFeetTouchEverySurface()
        {
            yield return SceneManager.LoadSceneAsync("City");
            yield return null;
            var host = Object.FindObjectOfType<GameHost>();
            var mover = Object.FindObjectOfType<PlayerMover>();
            var feet = mover.Visual.GetComponent<FootGrounding>();
            Assert.IsNotNull(feet, "The player's body is not grounded");
            foreach (var spot in Spots)
            {
                OwnedFleet.PlacePlayer(host, spot.x, spot.z);
                for (float end = Time.time + 1; Time.time < end;) yield return null;
                Assert.Less(Mathf.Abs(GroundProbe.Gap(mover.Visual.position)), Tolerance, "Feet off the ground: " + spot.place);
                Assert.Less(Mathf.Abs(feet.Gap), Tolerance, spot.place);
            }
        }
    }
}
