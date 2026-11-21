using UnityEditor.SceneManagement;
using UnityEngine;

namespace Steel.Editor.Showroom
{
    /// <summary>An empty lit scene with a concrete floor, used to review one asset at a time.</summary>
    internal static class ShowroomStage
    {
        internal const int Width = 1920, Height = 1080;

        internal static void Create()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CityGeometry.Root = new GameObject("Showroom").transform;
            CityLighting.Create();
            // Its own material, so the stage tiling never leaks into the city's concrete.
            Material floor = CityMaterials.Surface("concrete_floor_02", new Vector2(8, 8), "Showroom lantai",
                new Color(.5f, .52f, .54f));
            CityGeometry.Box("Floor", Vector3.down * .05f, new Vector3(30, .1f, 30), floor);
        }

        internal static string SheetPath(string name) => "../docs/design/sheets/" + name + ".png";
    }
}
