using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Steel.Services;
using Steel.Presentation.Input;
using Steel.Presentation.Player;
using Steel.Presentation.UI;
using Steel.Presentation.World;

namespace Steel.Editor
{
    public static class CityBuilder
    {
        [MenuItem("Steel/Generate City")]
        public static void Generate()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var layout = new CityLayout(new ResourceConfigSource());
            CityGeometry.Root = new GameObject("Kawasan Industri / Cakung").transform;
            CityLighting.Create(); StreetBuilder.Create(); BuildingBuilder.Create();
            YardBuilder.Create(layout); PileBuilder.Create(layout);
            var host = new GameObject("Game Services"); host.AddComponent<GameHost>();
            host.AddComponent<SliceInput>(); host.AddComponent<ScreenshotCapture>();
            Transform player = WorkerBuilder.Create(layout);
            var camera = new GameObject("Main Camera", typeof(Camera)); camera.tag = "MainCamera";
            camera.AddComponent<AudioListener>(); camera.GetComponent<Camera>().fieldOfView = 62;
            camera.GetComponent<Camera>().nearClipPlane = .1f; camera.GetComponent<Camera>().farClipPlane = 200;
            var data = camera.AddComponent<UniversalAdditionalCameraData>(); data.renderPostProcessing = true;
            data.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
            camera.AddComponent<OrbitCamera>().Target = player;
            new GameObject("HUD").AddComponent<CityHud>();
            var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            settings.FindProperty("activeInputHandler").intValue = 1; settings.ApplyModifiedPropertiesWithoutUndo();
            PlayerSettings.productName = "Steel Tycoon 3D";
            PlayerSettings.defaultScreenWidth = 1280; PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            EditorSceneManager.SaveScene(scene, "Assets/_Project/Scenes/City.unity");
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/_Project/Scenes/City.unity", true) };
            AssetDatabase.SaveAssets();
            Debug.Log("CITY_GENERATED: 8 piles, lapak, factory, URP PBR + ACES + bloom + SSAO");
        }
        [MenuItem("Steel/Build Linux Slice")]
        public static void BuildLinux()
        {
            var result = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { "Assets/_Project/Scenes/City.unity" },
                locationPathName = "Build/Linux/SteelTycoon.x86_64", target = BuildTarget.StandaloneLinux64,
                options = BuildOptions.None
            });
            if (result.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new System.Exception("Linux build failed: " + result.summary.result);
            Debug.Log("LINUX_BUILD_SUCCEEDED: " + result.summary.totalSize + " bytes");
        }
    }
}
