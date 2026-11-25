using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Steel.Presentation.World.Mobile
{
    public static class MobileView
    {
        public const string TrimLayer = "MobileTrim", PropLayer = "MobileProp";
        public static void Apply(Camera camera)
        {
            var distance = camera.layerCullDistances;
            distance[LayerMask.NameToLayer(TrimLayer)] = 24;
            distance[LayerMask.NameToLayer(PropLayer)] = 48;
            camera.layerCullDistances = distance; camera.layerCullSpherical = true;
            camera.GetUniversalAdditionalCameraData().renderPostProcessing = false;
            camera.farClipPlane = Mathf.Min(camera.farClipPlane, 150);
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Start()
        {
            if (!Application.isMobilePlatform || !Camera.main) return;
            Apply(Camera.main); QualitySettings.vSyncCount = 0; MobileQuality.Restore();
        }
    }
}
