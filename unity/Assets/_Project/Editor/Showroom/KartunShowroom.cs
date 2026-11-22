using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Steel.Editor.Showroom
{
    /// <summary>The cartoon cast as it stands in the generated city: player, cart, motor bak and the dealer exit.</summary>
    public static class KartunShowroom
    {
        [MenuItem("Steel/Showroom/Kartun")]
        public static void Cast()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            int w = ShowroomStage.Width, h = ShowroomStage.Height, tw = w / 2, th = h / 2;
            var camera = new ShowroomCamera(w, h, true);
            Vector3 player = Stand("Pemain", .8f), cart = Stand("Gerobak dorong", .6f), bike = Stand("Motor triseda", .8f);
            camera.Shoot(player, 200, 8, 3.2f, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            camera.Shoot(player, 200, 8, 3.2f, new RectInt(0, th, tw, th));
            camera.Shoot(Vector3.Lerp(player, cart, .5f), 150, 14, 5.5f, new RectInt(tw, th, tw, th));
            camera.Shoot(bike, 140, 12, 5, new RectInt(0, 0, tw, th));
            camera.Shoot(bike + new Vector3(-5, 0, 0), 250, 24, 14, new RectInt(tw, 0, tw, th));   // dealer to the road
            camera.Save(ShowroomStage.SheetPath("kartun"));
        }

        private static Vector3 Stand(string name, float height) =>
            GameObject.Find(name).transform.position + Vector3.up * height;
    }
}
