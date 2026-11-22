using Steel.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI.Boot
{
    /// <summary>Covers the menu while the city loads and shows how far it is.</summary>
    public sealed class LoadingView
    {
        private const float Width = 560;
        private readonly RectTransform bar;
        public Fader Screen { get; }
        public Text Caption { get; }

        internal LoadingView(RectTransform safe, BootTheme theme)
        {
            var root = BootKit.Stretch("Memuat", safe);
            BootKit.Fill(root, theme.Background, true);
            var centre = new Vector2(.5f, .5f);
            BootKit.Picture(theme.logo, root, centre, new Vector2(180, 180), new Vector2(0, 110));
            Caption = BootKit.Label(theme, root, MenuText.Loading(0), 30, theme.Text, centre, new Vector2(800, 48),
                new Vector2(0, -40), bold: true);
            var track = UiFactory.Rect("Jalur", root, centre, new Vector2(Width, 16), new Vector2(0, -92));
            BootKit.Fill(track, theme.Panel, false);
            bar = UiFactory.Rect("Isi", track, new Vector2(0, .5f), new Vector2(0, 16), Vector2.zero);
            bar.pivot = new Vector2(0, .5f);
            BootKit.Fill(bar, theme.Accent, false);
            Screen = Fader.On(root, false);
        }

        /// <summary>Progress from 0 to 1.</summary>
        public void Set(float progress)
        {
            Caption.text = MenuText.Loading(progress);
            bar.sizeDelta = new Vector2(Width * Mathf.Clamp01(progress), 16);
        }
    }
}
