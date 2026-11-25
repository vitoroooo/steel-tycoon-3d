using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Steel.Editor.Showroom
{
    /// <summary>The heavy fleet as it stands in the generated city: the truck pool, then each truck alone.</summary>
    public static class TruckShowroom
    {
        [MenuItem("Steel/Showroom/Truk")]
        public static void Pool()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            int w = ShowroomStage.Width, h = ShowroomStage.Height, tw = w / 2, th = h / 2;
            var camera = new ShowroomCamera(w, h, true);
            Vector3 engkel = Stand("Truk engkel"), tronton = Stand("Tronton"), trailer = Stand("Trailer flat-deck");
            camera.Shoot(tronton, 150, 12, 22, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            camera.Shoot(tronton + Vector3.forward * 2, 168, 16, 30, new RectInt(0, th, tw, th));
            camera.Shoot(engkel, 120, 10, 13, new RectInt(tw, th, tw, th));
            camera.Shoot(tronton + Vector3.forward * 2, 190, 24, 19, new RectInt(0, 0, w, th));
            camera.Save(ShowroomStage.SheetPath("kartun-truk-kota"));
        }

        private static Vector3 Stand(string name) => GameObject.Find(name).transform.position + Vector3.up * 1.5f;
    }
}
