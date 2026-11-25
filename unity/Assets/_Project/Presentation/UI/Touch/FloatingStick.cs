using UnityEngine;
using UnityEngine.EventSystems;

namespace Steel.Presentation.UI
{
    public sealed class FloatingStick : ThumbSurface
    {
        public RectTransform Ring, Thumb;
        private Vector2 origin;
        private RectTransform Area => (RectTransform)transform;
        private Vector2 Local(PointerEventData e)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Area, e.position, e.pressEventCamera, out var p);
            return p;
        }
        protected override void Begin(PointerEventData e)
        {
            origin = Local(e);
            Ring.anchorMin = Ring.anchorMax = new Vector2(.5f,.5f);
            Rect r = Area.rect; float rad = Ring.sizeDelta.x / 2;
            Ring.anchoredPosition = new Vector2(Mathf.Clamp(origin.x, r.xMin + rad, r.xMax - rad),
                Mathf.Clamp(origin.y, r.yMin + rad, r.yMax - rad));
        }
        protected override void Drag(PointerEventData e)
        {
            float travel = (Ring.sizeDelta.x - Thumb.sizeDelta.x) * .5f;
            var axis = Vector2.ClampMagnitude((Local(e) - origin) / travel, 1);
            Thumb.anchoredPosition = axis * travel; Controls.SetStick(axis);
        }
        protected override void End()
        {
            if (!Ring || !Thumb) return;
            Ring.anchorMin = Ring.anchorMax = Vector2.zero;
            Ring.anchoredPosition = new Vector2(122,104); Thumb.anchoredPosition = Vector2.zero;
            Controls.SetStick(Vector2.zero);
        }
    }
}
