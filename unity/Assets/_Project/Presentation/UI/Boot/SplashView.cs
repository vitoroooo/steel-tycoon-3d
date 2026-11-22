using Steel.Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Steel.Presentation.UI.Boot
{
    /// <summary>Opening card: logo, title and tagline. A tap anywhere moves on.</summary>
    public sealed class SplashView
    {
        public Fader Screen { get; }
        public Button Surface { get; }

        internal SplashView(RectTransform safe, BootTheme theme, UnityAction skip)
        {
            var root = BootKit.Stretch("Pembuka", safe);
            Surface = root.gameObject.AddComponent<Button>();
            Surface.targetGraphic = BootKit.Fill(root, Color.clear, true);
            Surface.transition = Selectable.Transition.None;
            Surface.onClick.AddListener(skip);
            var centre = new Vector2(.5f, .5f);
            bool logo = BootKit.Picture(theme.logo, root, centre, new Vector2(240, 240), new Vector2(0, 96));
            BootKit.Label(theme, root, theme.title, 68, theme.Accent, centre, new Vector2(1000, 90),
                new Vector2(0, logo ? -82 : 30), bold: true);
            BootKit.Label(theme, root, theme.tagline, 28, theme.Text, centre, new Vector2(1000, 44),
                new Vector2(0, logo ? -146 : -40));
            BootKit.Label(theme, root, MenuText.Skip, 22, theme.Muted, new Vector2(.5f, 0), new Vector2(600, 40),
                new Vector2(0, 56));
            Screen = Fader.On(root, true);
        }
    }
}
