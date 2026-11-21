using System.Collections;
using NUnit.Framework;
using Steel.PlayTests.Support;
using Steel.Presentation.Input;
using Steel.Presentation.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

namespace Steel.PlayTests.Mobile
{
    public sealed class LandscapeTouchTests
    {
        private static PointerEventData Touch(int id, Vector2 at) =>
            new PointerEventData(EventSystem.current) { pointerId = id, position = at };

        [UnityTest]
        public IEnumerator TwoThumbsStayIndependentAcrossCentreAndModal()
        {
            yield return OwnedFleet.LoadWealthyCity();
            var stick = Object.FindObjectOfType<FloatingStick>();
            var look = Object.FindObjectOfType<OrbitSurface>();
            var input = Object.FindObjectOfType<SliceInput>();
            var left = Touch(15, stick.transform.position);
            var right = Touch(81, look.transform.position);
            stick.OnPointerDown(left); look.OnPointerDown(right);
            left.position += Vector2.up * 80; stick.OnDrag(left);
            Assert.Greater(input.Move.y,.6f);
            right.position = left.position; right.delta = new Vector2(36,-9);
            look.OnDrag(right); look.OnPointerUp(left);
            look.OnDrag(right); Assert.Greater(input.TakeOrbit().x, 0);
            Assert.AreEqual(Vector2.zero, input.TakeOrbit(), "No stale delta next frame");
            look.OnPointerUp(right); Assert.Greater(input.Move.y,.6f);
            input.IsBlocked = true; input.IsBlocked = false;
            stick.OnDrag(left); Assert.AreEqual(Vector2.zero,input.Move, "Modal cancels captured sticks");
            look.OnDrag(right); Assert.AreEqual(Vector2.zero,input.TakeOrbit());
            stick.OnPointerDown(left); left.position -= Vector2.right * 90; stick.OnDrag(left);
            Assert.Less(input.Move.x,-.6f); stick.OnPointerUp(left);
            Assert.AreEqual(Vector2.zero,input.Move);
        }
        [UnityTest]
        public IEnumerator SameFractionOfScreenGivesSameRotationAtDifferentResolutions()
        {
            yield return OwnedFleet.LoadWealthyCity();
            var input = Object.FindObjectOfType<SliceInput>();
            input.AddOrbit(new Vector2(72,36),720); var first = input.TakeOrbit();
            input.AddOrbit(new Vector2(144,72),1440);
            Assert.AreEqual(first,input.TakeOrbit()); Assert.Greater(first.x, 10);
        }
    }
}
