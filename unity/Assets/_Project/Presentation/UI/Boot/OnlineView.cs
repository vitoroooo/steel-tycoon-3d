using Steel.Services.Remote;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI.Boot
{
    /// <summary>The "Main online" card over the start menu: server address, account token, and what the server said.</summary>
    public sealed class OnlineView
    {
        public Fader Screen { get; }
        public InputField Address { get; }
        public InputField Token { get; }
        public Button Go { get; }
        public Button Cancel { get; }
        public Text Message { get; }

        internal OnlineView(RectTransform safe, BootTheme theme, OnlineLogin login)
        {
            var root = BootKit.Stretch("Main Online", safe);
            BootKit.Fill(root, new Color(0, 0, 0, .6f), true);
            Vector2 centre = new Vector2(.5f, .5f), top = new Vector2(.5f, 1), bottom = new Vector2(.5f, 0);
            var card = UiFactory.Rect("Kartu Online", root, centre, new Vector2(760, 520), Vector2.zero);
            BootKit.Fill(card, theme.Panel, true);
            BootKit.Label(theme, card, OnlineLogin.Title, 30, theme.Accent, top, new Vector2(700, 44), new Vector2(0, -44), bold: true);
            Message = BootKit.Label(theme, card, login.Message, 21, theme.Text, top, new Vector2(700, 64), new Vector2(0, -104));
            Address = Field(theme, card, OnlineLogin.AddressHint, login.Account.origin, 253, false, -196);
            Token = Field(theme, card, OnlineLogin.TokenHint, login.Account.token, 43, true, -296);
            Cancel = BootKit.Button(theme, card, OnlineLogin.Cancel, 24, theme.Background, theme.Text, bottom,
                new Vector2(300, BootKit.Thumb), new Vector2(-170, 72), login.Back);
            Go = BootKit.Button(theme, card, OnlineLogin.Go, 26, theme.Accent, theme.AccentInk, bottom,
                new Vector2(300, BootKit.Thumb), new Vector2(170, 72), () => login.Submit(Address.text, Token.text));
            Screen = Fader.On(root, false);
        }

        public void Draw(OnlineLogin login)
        {
            Message.text = login.Message;
            Go.interactable = Cancel.interactable = login.Stage == LoginStage.Form;
        }

        private static InputField Field(BootTheme theme, Transform card, string hint, string value, int limit, bool secret, float y)
        {
            var box = UiFactory.Rect(hint, card, new Vector2(.5f, 1), new Vector2(700, 76), new Vector2(0, y));
            var field = box.gameObject.AddComponent<InputField>();
            field.targetGraphic = BootKit.Fill(box, theme.Background, true);
            field.textComponent = Line(theme, box, "", theme.Text);
            field.placeholder = Line(theme, box, hint, theme.Muted);
            field.characterLimit = limit; field.text = value ?? "";
            field.contentType = secret ? InputField.ContentType.Password : InputField.ContentType.Standard;
            field.shouldHideMobileInput = secret;
            return field;
        }

        private static Text Line(BootTheme theme, Transform box, string value, Color colour)
        {
            Text line = BootKit.Label(theme, box, value, 22, colour, new Vector2(.5f, .5f), new Vector2(660, 60),
                Vector2.zero, TextAnchor.MiddleLeft);
            line.resizeTextForBestFit = false; line.horizontalOverflow = HorizontalWrapMode.Overflow;
            line.supportRichText = false;
            return line;
        }
    }
}
