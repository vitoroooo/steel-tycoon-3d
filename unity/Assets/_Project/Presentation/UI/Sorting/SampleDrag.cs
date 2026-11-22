using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Steel.Presentation.UI
{
    public sealed class SampleDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        internal RectTransform[] Bins;
        internal Action<int> Drop;
        private RectTransform Card => (RectTransform)transform;
        private Vector2 start, pointerStart;
        private int? owner;
        public void OnBeginDrag(PointerEventData e)
        {
            if (owner.HasValue) return;
            owner = e.pointerId; start = Card.anchoredPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)Card.parent,
                e.position, e.pressEventCamera, out pointerStart);
            Card.SetAsLastSibling();
        }
        public void OnDrag(PointerEventData e)
        {
            if (owner != e.pointerId) return;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)Card.parent,
                e.position, e.pressEventCamera, out var local)) Card.anchoredPosition = start + local - pointerStart;
        }
        public void OnEndDrag(PointerEventData e)
        {
            if (owner != e.pointerId) return;
            owner = null; Card.anchoredPosition = start;
            for (int i = 0; i < Bins.Length; i++)
                if (RectTransformUtility.RectangleContainsScreenPoint(Bins[i], e.position, e.pressEventCamera))
                { Drop(i); return; }
        }
        private void OnDisable() { if (owner.HasValue) Card.anchoredPosition = start; owner = null; }
    }
}
