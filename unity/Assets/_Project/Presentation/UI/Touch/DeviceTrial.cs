using Steel.Presentation.World.Mobile;
using Steel.Services;
using UnityEngine;

namespace Steel.Presentation.UI
{
    internal sealed class DeviceTrial : MonoBehaviour
    {
        private FrameSample sample;
        private double last;
        private string profile;
        internal string Report { get; private set; } = "";
        internal bool Active => sample != null;
        internal int Remaining => Active ? Mathf.CeilToInt((float)(30-sample.Elapsed)) : 0;

        internal void Begin()
        {
            sample = new FrameSample(); profile = MobileQuality.Label;
            last = -1; Report = "";
        }
        private void Update()
        {
            if (!Active) return;
            if (profile != MobileQuality.Label) { Cancel(); return; }
            double now = Time.realtimeSinceStartupAsDouble;
            if (last >= 0) sample.Add(now-last);
            last = now;
            if (!sample.Finished) return;
            string p95 = sample.P95Ms() == 1000 ? "≥1000" : sample.P95Ms().ToString();
            Report = $"{SystemInfo.deviceModel} · {Screen.width}×{Screen.height} · {profile}\n" +
                $"{sample.Elapsed:F1} dtk · {sample.AverageFps:F1} FPS · P95 {p95} ms\n" +
                $"{sample.SlowFrames}/{sample.Frames} frame >50 ms · Terlama {sample.WorstMs:F0} ms";
            sample = null;
            GetComponent<CityHud>().NoticeText.text = "Uji selesai. Buka Buku usaha › Uji performa HP untuk hasilnya.";
        }
        private void Cancel()
        {
            if (!Active) return;
            sample = null; Report = "Uji terhenti. Gunakan satu profil grafis dan biarkan game terbuka.";
        }
        private void OnApplicationPause(bool paused) { if (paused) Cancel(); }
        private void OnApplicationFocus(bool focused) { if (!focused) Cancel(); }
    }
}
