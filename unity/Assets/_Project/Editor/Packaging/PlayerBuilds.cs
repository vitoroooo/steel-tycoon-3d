using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Steel.Editor.Packaging
{
    /// <summary>Command-line player builds. Run with -executeMethod Steel.Editor.Packaging.PlayerBuilds.Android (or .Linux).</summary>
    public static class PlayerBuilds
    {
        private const string Scene = "Assets/_Project/Scenes/City.unity";
        private const string AndroidId = "com.steeltycoon.sukabaja";
        private const AndroidSdkVersions OldestAndroid = AndroidSdkVersions.AndroidApiLevel24;   // Android 7: first with Vulkan

        [MenuItem("Steel/Build/Linux")]
        public static void Linux() => Build("Linux", "Build/Linux/SteelTycoon.x86_64", BuildTarget.StandaloneLinux64);

        /// <summary>A debug-signed 64-bit APK that installs straight onto a phone.</summary>
        [MenuItem("Steel/Build/Android APK")]
        public static void Android()
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, AndroidId);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = OldestAndroid;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            EditorUserBuildSettings.buildAppBundle = false;
            Build("Android", "Build/Android/SteelTycoon.apk", BuildTarget.Android);
        }

        private static void Build(string label, string path, BuildTarget target)
        {
            BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { Scene }, locationPathName = path, target = target, options = BuildOptions.None
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new System.Exception(label + " build failed: " + report.summary.result);
            Debug.Log(label.ToUpperInvariant() + "_BUILD_SUCCEEDED: " + report.summary.totalSize + " bytes");
        }
    }
}
