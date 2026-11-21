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
            scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = 0;
            hud.SafeRoot = new GameObject("Safe Area", typeof(RectTransform)).GetComponent<RectTransform>();
            hud.SafeRoot.SetParent(canvas.transform, false); hud.SafeRoot.gameObject.AddComponent<SafeAreaLayout>();
            hud.gameObject.AddComponent<GraphicRaycaster>();
            var events = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            var left = UiFactory.Panel("Usaha", hud.SafeRoot, new Vector2(0, 1),
                new Vector2(350, 125), new Vector2(195, -88));
            UiFactory.Text(left, "BAJA / TYCOON", 17, new Vector2(315, 25), new Vector2(0, 38)).color = UiFactory.Gold;
            hud.CashText = UiFactory.Text(left, "", 27, new Vector2(315, 40), new Vector2(0, 1));
            hud.LoadText = UiFactory.Text(left, "", 15, new Vector2(315, 27), new Vector2(0, -38));
            var right = UiFactory.Panel("Pasar", hud.SafeRoot, Vector2.one,
                new Vector2(315, 125), new Vector2(-177, -88));
            hud.MarketText = UiFactory.Text(right, "", 18, new Vector2(280, 105), Vector2.zero, TextAnchor.MiddleRight);
            hud.ActionText = TouchControls.Build(hud.SafeRoot);
            hud.VehicleButton = VehicleHud.Build(hud);
            var bottom = UiFactory.Panel("Petunjuk", hud.SafeRoot, new Vector2(.5f, 0),
                new Vector2(640, 110), new Vector2(0, 100));
            hud.PromptText = UiFactory.Text(bottom, "", 18, new Vector2(600, 85), Vector2.zero, TextAnchor.MiddleCenter);
            hud.NoticeText = UiFactory.Text(hud.SafeRoot, "", 18, new Vector2(800, 45), new Vector2(0, -145),
                TextAnchor.MiddleCenter);
            var help = UiFactory.Rect("Navigasi", hud.SafeRoot, new Vector2(.5f, 0), new Vector2(900, 64), new Vector2(0, 35));
            hud.RouteText = UiFactory.Text(help, "", 13, new Vector2(900, 64), Vector2.zero, TextAnchor.MiddleCenter);
            SaleDialog.Build(hud);
        }
    }
}
