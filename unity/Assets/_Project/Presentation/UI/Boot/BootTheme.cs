using System;
using UnityEngine;

namespace Steel.Presentation.UI.Boot
{
    /// <summary>
    /// Look of the start screens, read from Resources/Theme/Boot.json. Swap colours, wording and pictures
    /// there (pictures are Resources paths; leave empty for none) without touching the flow.
    /// </summary>
    [Serializable]
    public sealed class BootTheme
    {
        public const string Resource = "Theme/Boot";
        public string title = "STEEL TYCOON 3D", tagline = "";
        public string background = "#14244A", panel = "#1F3768", accent = "#FFC533", accentInk = "#14244A";
        public string text = "#FFFFFF", muted = "#B9C5DF", danger = "#E4553F";
        public string logo = "", hero = "", backdrop = "", font = "";
        public float splashSeconds = 1.8f;

        public Color Background => Parse(background);
        public Color Panel => Parse(panel);
        public Color Accent => Parse(accent);
        public Color AccentInk => Parse(accentInk);
        public Color Text => Parse(text);
        public Color Muted => Parse(muted);
        public Color Danger => Parse(danger);
        public float SplashSeconds => float.IsNaN(splashSeconds) ? 0 : Mathf.Clamp(splashSeconds, 0, 10);

        /// <summary>Keys missing from the file keep the defaults above; a missing file is the default theme.</summary>
        public static BootTheme Load() => Read(Resources.Load<TextAsset>(Resource)?.text);

        public static BootTheme Read(string json)
        {
            var theme = new BootTheme();
            if (!string.IsNullOrWhiteSpace(json)) JsonUtility.FromJsonOverwrite(json, theme);
            return theme;
        }

        /// <summary>Null when the theme names no picture or the picture is not in Resources.</summary>
        public static Texture2D Picture(string path) =>
            string.IsNullOrWhiteSpace(path) ? null : Resources.Load<Texture2D>(path);

        public Font Font
        {
            get
            {
                var custom = string.IsNullOrWhiteSpace(font) ? null : Resources.Load<Font>(font);
                return custom ? custom : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
        }

        private static Color Parse(string html) =>
            ColorUtility.TryParseHtmlString(html, out var colour) ? colour : Color.magenta;
    }
}
