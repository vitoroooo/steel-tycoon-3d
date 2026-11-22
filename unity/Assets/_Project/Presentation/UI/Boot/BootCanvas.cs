using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Steel.Presentation.UI.Boot
{
    /// <summary>Camera, canvas, safe area and touch input for the start screens; all children of the host.</summary>
    internal static class BootCanvas
    {
        internal static RectTransform Build(Transform host, BootTheme theme)
        {
            var camera = new GameObject("Kamera Pembuka", typeof(Camera)).GetComponent<Camera>();
            camera.transform.SetParent(host, false);
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = theme.Background;
            camera.cullingMask = 0;

            var canvas = new GameObject("Layar Pembuka", typeof(RectTransform)).AddComponent<Canvas>();
            canvas.transform.SetParent(host, false);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = .8f;
            canvas.gameObject.AddComponent<GraphicRaycaster>();

            // Colour and backdrop run under notches; everything tappable stays inside the safe area.
            BootKit.Fill(BootKit.Stretch("Latar", canvas.transform), theme.Background, false);
            var backdrop = BootKit.Picture(theme.backdrop, canvas.transform, new Vector2(.5f, .5f), Vector2.zero, Vector2.zero);
            if (backdrop)
            {
                var frame = (RectTransform)backdrop.transform.parent;
                frame.anchorMin = Vector2.zero; frame.anchorMax = Vector2.one;
                backdrop.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            }
            var safe = BootKit.Stretch("Safe Area", canvas.transform);
            safe.gameObject.AddComponent<SafeAreaLayout>();

            var events = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            events.transform.SetParent(host, false);
            events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            return safe;
        }
    }
}
