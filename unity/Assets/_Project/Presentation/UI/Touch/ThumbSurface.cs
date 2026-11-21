using Steel.Presentation.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Steel.Presentation.UI
{
    public abstract class ThumbSurface : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
        IDragHandler, IInitializePotentialDragHandler
    {
        protected SliceInput Controls;
        private int? owner;
        public void Bind(SliceInput controls) { Controls = controls; Controls.ResetTouches += Release; }
        public void OnInitializePotentialDrag(PointerEventData e) => e.useDragThreshold = false;
        public void OnPointerDown(PointerEventData e)
        {
            if (e.button != PointerEventData.InputButton.Left || Controls.IsBlocked || owner.HasValue) return;
            owner = e.pointerId; Begin(e);
        }
        public void OnDrag(PointerEventData e) { if (owner == e.pointerId && !Controls.IsBlocked) Drag(e); }
        public void OnPointerUp(PointerEventData e) { if (owner == e.pointerId) Release(); }
        private void Release() { owner = null; End(); }
        private void OnDisable() { if (Controls) Release(); }
        private void OnDestroy() { if (Controls) Controls.ResetTouches -= Release; }
        protected virtual void Begin(PointerEventData e) { }
        protected abstract void Drag(PointerEventData e);
        protected virtual void End() { }
    }
}
