using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Steel.Presentation.UI.Boot
{
    /// <summary>Themed building blocks shared by the start screens.</summary>
    internal static class BootKit
    {
        /// <summary>Smallest side of anything tappable, in 1280x720 canvas units (about 8 mm on a phone).</summary>
        internal const float Thumb = 88;

        internal static RectTransform Stretch(string name, Transform parent)
        {
            var rect = UiFactory.Rect(name, parent, Vector2.zero, Vector2.zero, Vector2.zero);
            rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
            return rect;
        }

        internal static Image Fill(RectTransform rect, Color colour, bool blocksTouch)
        {
            var image = rect.gameObject.AddComponent<Image>();
            image.color = colour; image.raycastTarget = blocksTouch; return image;
        }

        internal static Text Label(BootTheme theme, Transform parent, string value, int size, Color colour,
            Vector2 anchor, Vector2 box, Vector2 offset, TextAnchor align = TextAnchor.MiddleCenter, bool bold = false)
        {
            var label = UiFactory.Text(parent, value, size, box, offset, align);
            label.rectTransform.anchorMin = label.rectTransform.anchorMax = anchor;
            label.rectTransform.sizeDelta = box; label.rectTransform.anchoredPosition = offset;
            label.font = theme.Font; label.color = colour;
            label.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
            label.horizontalOverflow = HorizontalWrapMode.Wrap; label.verticalOverflow = VerticalWrapMode.Truncate;
            label.resizeTextForBestFit = true; label.resizeTextMinSize = Mathf.Min(size, 16); label.resizeTextMaxSize = size;
            return label;
        }

        internal static Button Button(BootTheme theme, Transform parent, string caption, int size, Color fill, Color ink,
            Vector2 anchor, Vector2 box, Vector2 offset, UnityAction action)
        {
            var rect = UiFactory.Rect(caption, parent, anchor, box, offset);
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = Fill(rect, fill, true);
            Label(theme, rect, caption, size, ink, new Vector2(.5f, .5f), box - new Vector2(24, 12), Vector2.zero, bold: true);
            button.onClick.AddListener(action);
            rect.gameObject.AddComponent<ThumbPress>().Face = rect;
            return button;
        }

        /// <summary>Shows a theme picture inside the box, keeping its proportions; null when there is none.</summary>
        internal static RawImage Picture(string path, Transform parent, Vector2 anchor, Vector2 box, Vector2 offset)
        {
            var texture = BootTheme.Picture(path);
            if (!texture) return null;
            var rect = UiFactory.Rect("Gambar " + path, parent, anchor, box, offset);
            var frame = UiFactory.Rect("Isi", rect, new Vector2(.5f, .5f), Vector2.zero, Vector2.zero);
            var image = frame.gameObject.AddComponent<RawImage>();
            image.texture = texture; image.raycastTarget = false;
            var fit = frame.gameObject.AddComponent<AspectRatioFitter>();
            fit.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            fit.aspectRatio = (float)texture.width / texture.height;
            return image;
        }
    }
}
