using UnityEngine;

namespace Steel.Presentation.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaLayout : MonoBehaviour
    {
        private Rect last;
        private Vector2 screen;
        public static Rect Normalize(Rect area, Vector2 size)
        {
            if (size.x <= 0 || size.y <= 0) return new Rect(0, 0, 1, 1);
            return Rect.MinMaxRect(Mathf.Clamp01(area.xMin / size.x), Mathf.Clamp01(area.yMin / size.y),
                Mathf.Clamp01(area.xMax / size.x), Mathf.Clamp01(area.yMax / size.y));
        }
        private void OnEnable() => Apply();
        private void Update()
        {
            if (last != Screen.safeArea || screen != new Vector2(Screen.width, Screen.height)) Apply();
        }
        private void Apply()
        {
            last = Screen.safeArea; screen = new Vector2(Screen.width, Screen.height);
            Rect area = Normalize(last, screen); var r = GetComponent<RectTransform>();
            r.anchorMin = area.min; r.anchorMax = area.max; r.offsetMin = r.offsetMax = Vector2.zero;
        }
    }
}
