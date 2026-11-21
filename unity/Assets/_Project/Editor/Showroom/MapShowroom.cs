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
            var camera = new ShowroomCamera(w, h, true);
            Vector3 centre = new Vector3(0, 0, 8);
            camera.Shoot(centre, 180, 89, 250, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            camera.Shoot(centre, 180, 89, 250, new RectInt(0, 0, w / 2, h));
            int tw = w / 4, th = h / 2;
            camera.Shoot(new Vector3(-13, 3, 38), 140, 4, 24, new RectInt(w / 2, th, tw, th));         // Pasar Lama row
            camera.Shoot(new Vector3(-23.3f, 2.2f, 44), 180, 5, 26, new RectInt(w / 2 + tw, th, tw, th));   // back alley
            camera.Shoot(new Vector3(-38.6f, 2.5f, 30), 180, 5, 36, new RectInt(w / 2, 0, tw, th));    // market lane and kampung
            camera.Shoot(new Vector3(36, 3, -6), 150, 5, 34, new RectInt(w / 2 + tw, 0, tw, th));     // workshops
            camera.Save(ShowroomStage.SheetPath("peta"));
        }

        /// <summary>Sky and ground review: towards the sun, away from it, and low over the open ground.</summary>
        [MenuItem("Steel/Showroom/Langit dan tanah")]
        public static void SkyAndGround()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            int w = ShowroomStage.Width, h = ShowroomStage.Height;
            var camera = new ShowroomCamera(w, h, true);
            Vector3 eye = new Vector3(0, 1.7f, -2);
            Vector3 toSun = -RenderSettings.sun.transform.forward;
            float bearing = Mathf.Atan2(toSun.x, toSun.z) * Mathf.Rad2Deg, height = Mathf.Asin(toSun.y) * Mathf.Rad2Deg;
            camera.Shoot(eye, bearing + 180, -height, 0, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            camera.Shoot(eye, bearing + 180, -height, 0, new RectInt(0, h / 2, w / 2, h / 2));     // sun should be mid-frame
            camera.Shoot(eye, bearing, -12, 0, new RectInt(w / 2, h / 2, w / 2, h / 2));       // away from the sun
            Vector3 puddle = GameObject.Find("Genangan").transform.position;
            camera.Shoot(puddle, bearing, 16, 9, new RectInt(0, 0, w / 2, h / 2));                      // a puddle, lit from behind
            camera.Shoot(new Vector3(30, 1.2f, -30), 320, 6, 0, new RectInt(w / 2, 0, w / 2, h / 2));   // open south-east
            camera.Save(ShowroomStage.SheetPath("langit-tanah"));
        }
    }
}
