using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Steel.Presentation.World.Mobile
{
    public static class MobileQuality
    {
        private const string Key = "graphics.mobile.mode";
        private static readonly string[] Names = { "Seimbang", "Ringan", "Tajam" };
        private static readonly float[] Scales = { .8f, .67f, 1 };
        public static string Label => Names[mode];
        private static int mode;
        public static void Restore() => Set(Mathf.Clamp(PlayerPrefs.GetInt(Key),0,Names.Length-1));
        public static void Cycle()
        {
            Set((mode + 1) % Names.Length);
            PlayerPrefs.SetInt(Key, mode); PlayerPrefs.Save();
        }
        private static void Set(int selection)
        {
            mode = selection;
            if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset p)
                p.renderScale = Scales[mode];
            Application.targetFrameRate = mode == 1 ? 30 : 60;
        }
    }
}
