using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Steel.Presentation.UI
{
    public static class HudLayout
    {
        public static void Build(CityHud hud)
        {
            var canvas = hud.gameObject.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = hud.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = .5f;
            hud.gameObject.AddComponent<GraphicRaycaster>();
            var events = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            var left = UiFactory.Panel("Usaha", canvas.transform, new Vector2(0, 1),
                new Vector2(350, 125), new Vector2(195, -88));
            UiFactory.Text(left, "BAJA / TYCOON", 17, new Vector2(315, 25), new Vector2(0, 38)).color = UiFactory.Gold;
            hud.CashText = UiFactory.Text(left, "", 27, new Vector2(315, 40), new Vector2(0, 1));
            hud.LoadText = UiFactory.Text(left, "", 15, new Vector2(315, 27), new Vector2(0, -38));
            var right = UiFactory.Panel("Pasar", canvas.transform, Vector2.one,
                new Vector2(315, 125), new Vector2(-177, -88));
            hud.MarketText = UiFactory.Text(right, "", 18, new Vector2(280, 105), Vector2.zero, TextAnchor.MiddleRight);
            hud.ActionText = TouchControls.Build(canvas.transform);
            var bottom = UiFactory.Panel("Petunjuk", canvas.transform, new Vector2(.5f, 0),
                new Vector2(640, 110), new Vector2(0, 100));
            hud.PromptText = UiFactory.Text(bottom, "", 18, new Vector2(600, 85), Vector2.zero, TextAnchor.MiddleCenter);
            hud.NoticeText = UiFactory.Text(canvas.transform, "", 18, new Vector2(800, 45), new Vector2(0, -145),
                TextAnchor.MiddleCenter);
            var help = UiFactory.Rect("Navigasi", canvas.transform, new Vector2(.5f, 0), new Vector2(900, 44), new Vector2(0, 22));
            hud.RouteText = UiFactory.Text(help, "", 14, new Vector2(900, 44), Vector2.zero, TextAnchor.MiddleCenter);
            SaleDialog.Build(hud);
        }
    }
}
