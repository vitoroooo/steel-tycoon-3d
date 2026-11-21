using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Steel.Presentation.Player;

namespace Steel.PlayTests
{
    public sealed class CameraFramingTests
    {
        [UnityTest] public IEnumerator ThinOffAxisPoleDoesNotCrossCentralFraming()
        {
            var pole = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pole.transform.position = new Vector3(149.2f, 3, 145);
            pole.transform.localScale = new Vector3(.08f, 6, .08f); Physics.SyncTransforms();
            Vector3 focus = new Vector3(150, 3, 150);
            Vector3 camera = CameraCollision.Resolve(focus, new Vector3(0, 1, -8));
            Vector3 line = camera - focus;
            float t = Mathf.Clamp01(Vector3.Dot(pole.transform.position - focus, line) / line.sqrMagnitude);
            Assert.Greater(Vector3.Distance(pole.transform.position, focus + t * line), 1.1f);
            Assert.Greater(line.magnitude, 7); Object.Destroy(pole); yield return null;
        }
        [Test] public void ClearViewKeepsDesiredCameraPosition()
        {
            var focus = new Vector3(180, 3, 180); var offset = new Vector3(0, 1, -8);
            Assert.AreEqual(focus + offset, CameraCollision.Resolve(focus, offset));
        }
    }
}
