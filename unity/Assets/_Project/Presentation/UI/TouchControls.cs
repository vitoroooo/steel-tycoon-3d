using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.OnScreen;

namespace Steel.Presentation.UI
{
    public static class TouchControls
    {
        private static Sprite Circle()
        {
            var tex = new Texture2D(64, 64);
            for (int y = 0; y < 64; y++)
                for (int x = 0; x < 64; x++)
                    tex.SetPixel(x, y, Vector2.Distance(new Vector2(x, y), new Vector2(31.5f, 31.5f)) < 31
                        ? Color.white : Color.clear);
            tex.Apply(); return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(.5f, .5f));
        }
        public static Text Build(Transform canvas)
        {
            var background = UiFactory.Panel("Joystick", canvas, Vector2.zero, new Vector2(170, 170), new Vector2(125, 125));
            background.GetComponent<Image>().sprite = Circle();
            background.GetComponent<Image>().color = new Color(.05f, .08f, .1f, .65f);
            var handle = UiFactory.Panel("Gerak", background, new Vector2(.5f, .5f), new Vector2(78, 78), Vector2.zero);
            handle.GetComponent<Image>().sprite = Circle(); handle.GetComponent<Image>().color = new Color(.9f, .85f, .68f, .8f);
            var stick = handle.gameObject.AddComponent<OnScreenStick>();
            stick.controlPath = "<Gamepad>/leftStick"; stick.movementRange = 55;
            UiFactory.Text(background, "JALAN", 13, new Vector2(160, 24), new Vector2(0, -100), TextAnchor.MiddleCenter);
            var button = UiFactory.Panel("Interaksi", canvas, new Vector2(1, 0), new Vector2(180, 70), new Vector2(-130, 105));
            button.GetComponent<Image>().color = UiFactory.Gold;
            var press = button.gameObject.AddComponent<OnScreenButton>(); press.controlPath = "<Gamepad>/buttonSouth";
            var text = UiFactory.Text(button, "INTERAKSI", 18, new Vector2(180, 70), Vector2.zero, TextAnchor.MiddleCenter);
            text.color = UiFactory.Ink; text.fontStyle = FontStyle.Bold; return text;
        }
    }
}
