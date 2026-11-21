using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Steel.Editor.Showroom
{
    /// <summary>Survey sheet of the generated city: the plan from above, then four views at walking height.</summary>
    public static class MapShowroom
    {
        [MenuItem("Steel/Showroom/Peta")]
        public static void Survey()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            int w = ShowroomStage.Width, h = ShowroomStage.Height;
            var camera = new ShowroomCamera(w, h);
            Vector3 centre = new Vector3(0, 0, 8);
            camera.Shoot(centre, 180, 89, 250, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            camera.Shoot(centre, 180, 89, 250, new RectInt(0, 0, w / 2, h));
            int tw = w / 4, th = h / 2;
            camera.Shoot(new Vector3(-13, 3, 38), 140, 4, 24, new RectInt(w / 2, th, tw, th));         // Pasar Lama row
            camera.Shoot(new Vector3(-23.3f, 2.2f, 44), 180, 5, 26, new RectInt(w / 2 + tw, th, tw, th));   // back alley
            camera.Shoot(new Vector3(-36, 3, 40), 210, 4, 30, new RectInt(w / 2, 0, tw, th));         // market lane
            camera.Shoot(new Vector3(36, 3, -6), 150, 5, 34, new RectInt(w / 2 + tw, 0, tw, th));     // workshops
            camera.Save(ShowroomStage.SheetPath("peta"));
        }
    }
}
