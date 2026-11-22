using System;
using Steel.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI.Boot
{
    /// <summary>
    /// Start menu for two thumbs in landscape: the play button sits under the right thumb, the rarely used
    /// buttons under the left, all in the lower half. Starting over is kept away from the play button.
    /// </summary>
    public sealed class MenuView
    {
        public Fader Screen { get; }
        public Button Play { get; }
        public Button NewCareer { get; }
        public Button Graphics { get; }
        public Text Card { get; }

        internal MenuView(RectTransform safe, BootTheme theme, MenuFlow flow, GraphicsOption graphics)
        {
            var root = BootKit.Stretch("Menu Awal", safe);
            Vector2 topLeft = new Vector2(0, 1), bottomLeft = Vector2.zero, bottomRight = new Vector2(1, 0);
            bool logo = BootKit.Picture(theme.logo, root, topLeft, new Vector2(112, 112), new Vector2(88, -88));
            float titleX = logo ? 164 : 32;
            BootKit.Label(theme, root, theme.title, 46, theme.Accent, topLeft, new Vector2(560, 60),
                new Vector2(titleX + 280, -70), TextAnchor.MiddleLeft, true);
            BootKit.Label(theme, root, theme.tagline, 24, theme.Text, topLeft, new Vector2(560, 36),
                new Vector2(titleX + 280, -116), TextAnchor.MiddleLeft);
            BootKit.Label(theme, root, "v" + Application.version, 18, theme.Muted, Vector2.one, new Vector2(160, 30),
                new Vector2(-112, -40), TextAnchor.MiddleRight);
            BootKit.Picture(theme.hero, root, new Vector2(0, .5f), new Vector2(420, 360), new Vector2(250, -10));

            var card = UiFactory.Rect("Kartu Karier", root, bottomRight, new Vector2(400, 190), new Vector2(-232, 263));
            BootKit.Fill(card, theme.Panel, false);
            BootKit.Label(theme, card, MenuText.CardTitle(flow.Career.State), 22, theme.Accent, new Vector2(.5f, 1),
                new Vector2(360, 34), new Vector2(0, -32), TextAnchor.MiddleLeft, true);
            Card = BootKit.Label(theme, card, MenuText.CardBody(flow.Career, DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                24, theme.Text, new Vector2(.5f, 0), new Vector2(360, 124), new Vector2(0, 72), TextAnchor.UpperLeft);
            Play = BootKit.Button(theme, root, MenuText.Play(flow.Career.State), 34, theme.Accent, theme.AccentInk,
                bottomRight, new Vector2(400, 112), new Vector2(-232, 88), flow.Play);

            float x = 32;
            if (flow.OffersNewCareer)
            {
                NewCareer = BootKit.Button(theme, root, MenuText.NewCareer, 22, theme.Panel, theme.Text, bottomLeft,
                    new Vector2(220, BootKit.Thumb), new Vector2(x + 110, 76), flow.AskNewCareer);
                x += 236;
            }
            if (graphics != null)
            {
                Graphics = BootKit.Button(theme, root, MenuText.Graphics(graphics.Label), 22, theme.Panel, theme.Text,
                    bottomLeft, new Vector2(240, BootKit.Thumb), new Vector2(x + 120, 76), () => { });
                var caption = Graphics.GetComponentInChildren<Text>();
                Graphics.onClick.AddListener(() => { graphics.Cycle(); caption.text = MenuText.Graphics(graphics.Label); });
            }
            Screen = Fader.On(root, false);
        }
    }
}
