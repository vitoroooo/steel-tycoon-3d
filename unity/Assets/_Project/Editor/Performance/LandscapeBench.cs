using System;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Steel.Editor.Performance
{
    public static class LandscapeBench
    {
        public static void Run() => Measure(false);
        public static void Mobile() => Measure(true);
        private static void Measure(bool mobile)
        {
            var scene = EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            Camera cam = Camera.main;
            var original = QualitySettings.renderPipeline;
            if (mobile)
            {
                QualitySettings.renderPipeline = MobilePipeline.Prepare();
                MobileDetailLayers.Apply(scene);
                Presentation.World.Mobile.MobileView.Apply(cam);
            }
            var target = RenderTexture.GetTemporary(960, 540, 24);
            cam.targetTexture = target;
            var readback = new Texture2D(1, 1, TextureFormat.RGB24, false);
            try
            {
                Sample(cam, readback, "lapak", new Vector3(-16,3,-1), 0);
                Sample(cam, readback, "pasar", new Vector3(-8,3,28), -32);
                Sample(cam, readback, "smelter", new Vector3(19,3,-24), 153);
            }
            finally
            {
                cam.targetTexture = null; RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(target); UnityEngine.Object.DestroyImmediate(readback);
                QualitySettings.renderPipeline = original;
            }
        }
        private static void Sample(Camera cam, Texture2D readback, string name, Vector3 eye, float yaw)
        {
            cam.transform.SetPositionAndRotation(eye, Quaternion.Euler(4, yaw, 0));
            var times = new double[11];
            var watch = new Stopwatch();
            for (int i = -3; i < times.Length; i++)
            {
                watch.Restart(); cam.Render(); RenderTexture.active = cam.targetTexture;
                readback.ReadPixels(new Rect(0,0,1,1), 0,0, false);
                watch.Stop();
                if (i >= 0) times[i] = watch.Elapsed.TotalMilliseconds;
            }
            Array.Sort(times);
            Debug.Log($"CITY_BENCH {name} render_ms_p50={times[5]:F1} renderer={SystemInfo.graphicsDeviceName}");
        }
    }
}
