using NUnit.Framework;
using UnityEngine;
using Steel.Presentation.UI;

namespace Steel.Tests.Fleet
{
    public sealed class SafeAreaTests
    {
        [Test] public void NotchedSafeAreaMapsToCanvasAnchors()
        {
            Rect r = SafeAreaLayout.Normalize(new Rect(40, 20, 1200, 680), new Vector2(1280, 720));
            Assert.AreEqual(40f / 1280, r.xMin, .0001f); Assert.AreEqual(20f / 720, r.yMin, .0001f);
            Assert.AreEqual(1240f / 1280, r.xMax, .0001f); Assert.AreEqual(700f / 720, r.yMax, .0001f);
        }
        [Test] public void ZeroScreenFallsBackToFullArea() =>
            Assert.AreEqual(new Rect(0, 0, 1, 1), SafeAreaLayout.Normalize(new Rect(), Vector2.zero));
        [Test] public void OutOfBoundsInsetsAreClamped() =>
            Assert.AreEqual(new Rect(0, 0, 1, 1),
                SafeAreaLayout.Normalize(new Rect(-10, -10, 1500, 900), new Vector2(1280, 720)));
    }
}
