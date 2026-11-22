using NUnit.Framework;
using Steel.Presentation.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Steel.PlayTests.Support
{
    internal static class SortGesture
    {
        internal static void Drop(CityHud hud, string bin)
        {
            Canvas.ForceUpdateCanvases();
            var desk = hud.SafeRoot.Find("Meja sortir"); Assert.IsTrue(desk.gameObject.activeSelf);
            var sample = desk.Find("Sampel").GetComponent<SampleDrag>();
            var e = new PointerEventData(EventSystem.current) { pointerId = 1, position = sample.transform.position };
            sample.OnBeginDrag(e); e.position = desk.Find(bin).position; sample.OnDrag(e); sample.OnEndDrag(e);
        }
    }
}
