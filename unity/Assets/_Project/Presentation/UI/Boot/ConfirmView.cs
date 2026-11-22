using Steel.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI.Boot
{
    /// <summary>
    /// "Karier baru" replaces the saved career, so it asks first. Both answers sit above the menu's bottom
    /// row, so a second tap where "Karier baru" was cannot confirm; "Batal" is on the right-thumb side.
    /// </summary>
    public sealed class ConfirmView
    {
        public Fader Screen { get; }
        public Button Yes { get; }
        public Button No { get; }
        public Text Body { get; }

        internal ConfirmView(RectTransform safe, BootTheme theme, MenuFlow flow)
        {
            var root = BootKit.Stretch("Konfirmasi Karier Baru", safe);
            var shade = theme.Background; shade.a = .82f;
            BootKit.Fill(root, shade, true);
            var panel = UiFactory.Rect("Dialog", root, new Vector2(.5f, 0), new Vector2(680, 330), new Vector2(0, 295));
            BootKit.Fill(panel, theme.Panel, false);
            Body = BootKit.Label(theme, panel, MenuText.Confirm(flow.Career), 25, theme.Text, new Vector2(.5f, 1),
                new Vector2(620, 170), new Vector2(0, -105));
            Yes = BootKit.Button(theme, panel, MenuText.ConfirmYes, 24, theme.Danger, theme.Text, new Vector2(0, 0),
                new Vector2(290, 96), new Vector2(175, 66), flow.ConfirmNewCareer);
            No = BootKit.Button(theme, panel, MenuText.ConfirmNo, 28, theme.Accent, theme.AccentInk, new Vector2(1, 0),
                new Vector2(290, 96), new Vector2(-175, 66), flow.Back);
            Screen = Fader.On(root, false);
        }
    }
}
