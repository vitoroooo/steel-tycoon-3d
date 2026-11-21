using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Steel.Editor.Showroom
{
    /// <summary>Review sheet of the smelter as it stands in the city: overview, gate, furnace, yard, stacks, plan.</summary>
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
    }
}
