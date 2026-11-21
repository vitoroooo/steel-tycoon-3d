using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using Steel.Presentation.Input;

namespace Steel.Presentation.UI
{
    public static class HudLayout
    {
        public static void Build(CityHud hud, SliceInput controls)
        {
            var canvas = hud.gameObject.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = hud.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = .8f;
            hud.SafeRoot = new GameObject("Safe Area", typeof(RectTransform)).GetComponent<RectTransform>();
            hud.SafeRoot.SetParent(canvas.transform, false); hud.SafeRoot.gameObject.AddComponent<SafeAreaLayout>();
            hud.gameObject.AddComponent<GraphicRaycaster>();
            var events = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            hud.ActionText = TouchControls.Build(hud, controls);
            var left = UiFactory.Panel("Usaha", hud.SafeRoot, new Vector2(0, 1),
                new Vector2(258, 72), new Vector2(144, -48));
            left.GetComponent<Image>().raycastTarget = false;
            hud.CashText = UiFactory.Text(left, "", 25, new Vector2(232, 36), new Vector2(0, 13));
            hud.LoadText = UiFactory.Text(left, "", 16, new Vector2(232, 25), new Vector2(0, -18));
            var right = UiFactory.Panel("Pasar", hud.SafeRoot, Vector2.one,
                new Vector2(201, 65), new Vector2(-115, -45));
            right.GetComponent<Image>().raycastTarget = false;
            hud.MarketText = UiFactory.Text(right, "", 16, new Vector2(177, 55), Vector2.zero, TextAnchor.MiddleRight);
            if (Application.isMobilePlatform) FrameReadout.Build(hud.SafeRoot);
            var bottom = UiFactory.Panel("Petunjuk", hud.SafeRoot, new Vector2(.5f, 0),
                new Vector2(420, 49), new Vector2(0, 89));
            bottom.GetComponent<Image>().raycastTarget = false;
            hud.PromptText = UiFactory.Text(bottom, "", 17, new Vector2(404, 47), Vector2.zero, TextAnchor.MiddleCenter);
            hud.NoticeText = UiFactory.Text(hud.SafeRoot, "", 17, new Vector2(680, 48), new Vector2(0, 149),
                TextAnchor.MiddleCenter);
            hud.NoticeText.rectTransform.anchorMin = hud.NoticeText.rectTransform.anchorMax = new Vector2(.5f, 0);
            var help = UiFactory.Rect("Navigasi", hud.SafeRoot, new Vector2(.5f, 0), new Vector2(590, 48), new Vector2(0, 33));
            hud.RouteText = UiFactory.Text(help, "", 15, new Vector2(590, 48), Vector2.zero, TextAnchor.MiddleCenter);
            SaleDialog.Build(hud);
        }
    }
}
