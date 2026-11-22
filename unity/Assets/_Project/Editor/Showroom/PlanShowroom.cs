using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Steel.Editor.Showroom
{
    /// <summary>Review sheet of greater Sukabaja: the whole plan from above, then streets, roundabout and river at eye level.</summary>
    public static class PlanShowroom
    {
        [MenuItem("Steel/Showroom/Peta besar")]
        public static void Survey()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            int w = ShowroomStage.Width, h = ShowroomStage.Height, tw = w / 4, th = h / 2;
            var camera = new ShowroomCamera(w, h, true);
            camera.Shoot(Vector3.zero, 180, 89, 1050, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            RenderSettings.fog = false;   // from this high the haze would wash the plan out
            camera.Shoot(Vector3.zero, 180, 89, 1050, new RectInt(0, 0, w / 2, h));
            RenderSettings.fog = true;
            camera.Shoot(new Vector3(-20, 1, 150), 200, 14, 60, new RectInt(w / 2, th, tw, th));        // Bundaran Tugu Baja, Pusat Kota
            camera.Shoot(new Vector3(-113, 1, 51), 150, 20, 55, new RectInt(w / 2 + tw, th, tw, th));   // Kampung Rawa
            camera.Shoot(new Vector3(170, 1, 60), 215, 16, 70, new RectInt(w / 2, 0, tw, th));          // Kawasan Industri
            camera.Shoot(new Vector3(-90, 1, -112), 30, 18, 70, new RectInt(w / 2 + tw, 0, tw, th));    // player plots in the south
            camera.Save(ShowroomStage.SheetPath("peta-besar"));
        }
    }
}
