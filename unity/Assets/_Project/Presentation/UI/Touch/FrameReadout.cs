using Steel.Presentation.World.Mobile;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    public sealed class FrameReadout : MonoBehaviour
    {
        private Text caption;
        private double start;
        private int frames;
        internal static void Build(RectTransform root)
        {
            var panel = UiFactory.Panel("Profil grafis",root,Vector2.one,new Vector2(201,60),new Vector2(-115,-114));
            var readout = panel.gameObject.AddComponent<FrameReadout>();
            readout.caption = UiFactory.Text(panel,"",18,new Vector2(193,56),Vector2.zero,TextAnchor.MiddleCenter);
            var button = panel.gameObject.AddComponent<Button>();
            button.onClick.AddListener(() => { MobileQuality.Cycle(); readout.ResetSample(); });
            readout.ResetSample();
        }
        private void OnApplicationPause(bool paused) { if (!paused && caption) ResetSample(); }
        private void ResetSample()
        {
            frames = 0; start = Time.realtimeSinceStartupAsDouble;
            caption.text = "Grafis: " + MobileQuality.Label + "  ›";
        }
        private void Update()
        {
            frames++; double now = Time.realtimeSinceStartupAsDouble, span = now - start;
            if (span < 2) return;
            caption.text = "Grafis: " + MobileQuality.Label + "  ›\n" + (frames/span).ToString("N0") + " FPS";
            frames = 0; start = now;
        }
    }
}
