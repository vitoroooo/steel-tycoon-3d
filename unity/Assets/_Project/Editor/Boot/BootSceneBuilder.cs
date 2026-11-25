using Steel.Editor.Packaging;
using Steel.Presentation.UI.Boot;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Steel.Editor.Boot
{
    /// <summary>Rebuilds the opening scene: one object that draws splash and start menu at run time.</summary>
    public static class BootSceneBuilder
    {
        [MenuItem("Steel/Generate Boot")]
        public static void Generate()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("Layar Awal").AddComponent<BootScreen>();
            EditorSceneManager.SaveScene(scene, BuildScenes.Boot);
            BuildScenes.Register();
            AssetDatabase.SaveAssets();
            Debug.Log("BOOT_GENERATED: splash + menu awal, lalu " + BootScreen.City);
        }
    }
}
