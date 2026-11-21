using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    public static class UiFactory
    {
        public static readonly Color Ink = new Color(.025f, .045f, .065f, .94f);
        public static readonly Color Gold = new Color(1, .65f, .16f);
        public static RectTransform Rect(string name, Transform parent, Vector2 anchor, Vector2 size, Vector2 offset)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>(); rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = anchor; rect.sizeDelta = size; rect.anchoredPosition = offset;
            return rect;
        }
        public static RectTransform Panel(string name, Transform parent, Vector2 anchor, Vector2 size, Vector2 offset)
        {
            var rect = Rect(name, parent, anchor, size, offset);
            rect.gameObject.AddComponent<Image>().color = Ink; return rect;
        }
        public static Text Text(Transform parent, string value, int fontSize, Vector2 size, Vector2 offset,
            TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            var rect = Rect(value, parent, new Vector2(.5f, .5f), size, offset);
            var text = rect.gameObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); text.text = value;
            text.fontSize = fontSize; text.color = Color.white; text.alignment = alignment;
            text.raycastTarget = false; return text;
        }
        public static Button Button(Transform parent, string caption, Vector2 size, Vector2 offset,
            UnityEngine.Events.UnityAction action)
        {
            var rect = Panel(caption, parent, new Vector2(.5f, .5f), size, offset);
            rect.GetComponent<Image>().color = Gold;
            var text = Text(rect, caption, 17, size, Vector2.zero, TextAnchor.MiddleCenter);
            text.color = Ink; text.fontStyle = FontStyle.Bold;
            var button = rect.gameObject.AddComponent<Button>(); button.onClick.AddListener(action); return button;
        }
    }
}
