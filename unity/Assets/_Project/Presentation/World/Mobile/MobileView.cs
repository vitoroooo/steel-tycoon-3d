using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

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
            if (!Application.isMobilePlatform) return;
            QualitySettings.vSyncCount = 0; MobileQuality.Restore();
            Follow();
        }
        /// <summary>Trims the game camera of every scene: it arrives after the start menu, not with the first scene.</summary>
        public static void Follow()
        {
            SceneManager.sceneLoaded -= Fit; SceneManager.sceneLoaded += Fit;
            Fit(default, default);
        }
        public static void Unfollow() => SceneManager.sceneLoaded -= Fit;
        private static void Fit(Scene scene, LoadSceneMode mode) { if (Camera.main) Apply(Camera.main); }
    }
}
