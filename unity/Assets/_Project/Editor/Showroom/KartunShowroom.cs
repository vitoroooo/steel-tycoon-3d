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
            float front = GameObject.Find("Pemain").transform.eulerAngles.y - 30;   // three-quarter view of the face
            camera.Shoot(player, front, 8, 3.2f, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            camera.Shoot(player, front, 8, 3.2f, new RectInt(0, th, tw, th));
            camera.Shoot(Vector3.Lerp(player, cart, .5f), front + 60, 14, 5.5f, new RectInt(tw, th, tw, th));
            camera.Shoot(bike, 140, 12, 5, new RectInt(0, 0, tw, th));
            camera.Shoot(bike + new Vector3(-5, 0, 0), 250, 24, 14, new RectInt(tw, 0, tw, th));   // dealer to the road
            camera.Save(ShowroomStage.SheetPath("kartun"));
        }

        [MenuItem("Steel/Showroom/Kartun pikap")]
        public static void Pikap()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            int w = ShowroomStage.Width, h = ShowroomStage.Height, tw = w / 2;
            var camera = new ShowroomCamera(w, h, true);
            Vector3 truck = Stand("Pikap bekas", .9f);
            camera.Shoot(truck, 140, 10, 9, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            camera.Shoot(truck, 178, 12, 10, new RectInt(0, 0, tw, h));
            camera.Shoot(truck, 208, 14, 10, new RectInt(tw, 0, tw, h));
            camera.Save(ShowroomStage.SheetPath("kartun-pikap"));
        }

        private static Vector3 Stand(string name, float height) =>
            GameObject.Find(name).transform.position + Vector3.up * height;
    }
}
