using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Steel.Editor.Showroom
{
    /// <summary>Review sheets of the two industrial sites as they stand in the city: the smelter and the demolition project.</summary>
    public static class SmelterShowroom
    {
        [MenuItem("Steel/Showroom/Pabrik peleburan")]
        public static void Smelter()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            foreach (var particles in Object.FindObjectsOfType<ParticleSystem>()) particles.Simulate(14, true, true);
            int w = ShowroomStage.Width, h = ShowroomStage.Height, tw = w / 3, th = h / 2;
            var camera = new ShowroomCamera(w, h, true);
            Vector3 site = new Vector3(32, 6, -44);
            camera.Shoot(site, 310, 12, 70, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            camera.Shoot(site, 310, 12, 70, new RectInt(0, th, tw * 2, th));                           // from the crossing
            camera.Shoot(new Vector3(20, 2.5f, -26), 350, 6, 20, new RectInt(tw * 2, th, tw, th));     // gate and weighbridge
            camera.Shoot(new Vector3(41, 3.5f, -52), 262, 6, 11, new RectInt(0, 0, tw, th));           // into the furnace bay
            camera.Shoot(new Vector3(23, 3.5f, -51), 20, 14, 30, new RectInt(tw, 0, tw, th));          // scrap bays and gantry
            camera.Shoot(new Vector3(48, 30, -30), 305, -4, 62, new RectInt(tw * 2, 0, tw, th));       // stacks and their smoke
            camera.Save(ShowroomStage.SheetPath("pabrik"));
        }

        [MenuItem("Steel/Showroom/Proyek bongkaran")]
        public static void Demolition()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            int w = ShowroomStage.Width, h = ShowroomStage.Height, tw = w / 3, th = h / 2;
            var camera = new ShowroomCamera(w, h, true);
            Vector3 wreck = new Vector3(-28, 4, -42);
            foreach (float yaw in new[] { 20f, 120, 200 })   // textures and shaders load on the first frames that need them
                camera.Shoot(wreck, yaw, 30, 60, new RectInt(0, 0, 1, 1));
            camera.Shoot(wreck, 20, 12, 38, new RectInt(0, th, tw, th));                                    // from the gate
            camera.Shoot(new Vector3(-24, 3.5f, -42), 125, 14, 30, new RectInt(tw, th, tw, th));            // the broken east end
            camera.Shoot(new Vector3(-17, 1.8f, -43), 165, 8, 13, new RectInt(tw * 2, th, tw, th));         // the breaker at work
            camera.Shoot(new Vector3(-33, 1, -25.5f), 172, 14, 10, new RectInt(0, 0, tw, th));              // salvage for resale
            camera.Shoot(new Vector3(-42, .8f, -60), 40, 20, 17, new RectInt(tw, 0, tw, th));               // the remains next door
            camera.Shoot(new Vector3(-30, 4, -45), 200, 42, 70, new RectInt(tw * 2, 0, tw, th));            // the whole site
            camera.Save(ShowroomStage.SheetPath("bongkaran"));
        }
    }
}
