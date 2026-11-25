using UnityEngine;
using UnityEngine.EventSystems;

namespace Steel.Presentation.UI
{
    public sealed class ThumbPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public RectTransform Face;
        public void OnPointerDown(PointerEventData e) { Face.localScale = Vector3.one * .91f; }
        public void OnPointerUp(PointerEventData e) { Face.localScale = Vector3.one; }
        private void OnDisable() { if (Face) Face.localScale = Vector3.one; }
    }
}
