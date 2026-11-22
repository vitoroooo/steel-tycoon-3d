using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Steel.Presentation.Vehicles.Haul;
using Steel.Presentation.World.Yard;
using Steel.Services.Map;

namespace Steel.Editor.Showroom
{
    /// <summary>One sheet of truck hauling as built: the mill's weighbridge, and the player's yard with stock and loader.</summary>
    public static class HaulShowroom
    {
        private static readonly float[] Sides = { -35, 35 };
        private static readonly float[] DemoTons = { 40, 12, 22, 5 };

        [MenuItem("Steel/Showroom/Muat bongkar")]
        public static void Sheet()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            var works = Object.FindObjectOfType<YardWorks>();
            for (int grade = 0; grade < works.Heaps.Length; grade++)   // stock as the yard would show it mid-career
            {
                float size = 5.5f * Mathf.Pow(DemoTons[grade] / 60, 1 / 3f);
                works.Heaps[grade].localScale = new Vector3(size * 1.5f, size, size * 1.2f);
            }
            var caller = works.GetComponent<YardCaller>();   // the collector as he stands reading the scale
            caller.Collector.localPosition = caller.Scale;
            caller.Arms.Hold(caller.GripLeft, caller.GripRight); caller.Arms.Snap();
            MapPlan plan = MapPlan.Load();
            MapSite[] sites = { plan.sites.First(s => s.kind == MapSite.MillGate), plan.sites.First(s => s.kind == MapSite.PlayerLapak) };
            int w = ShowroomStage.Width, h = ShowroomStage.Height, tileW = w / Sides.Length, tileH = h / sites.Length;
            var camera = new ShowroomCamera(w, h, true);
            RenderSettings.fog = false;
            for (int row = 0; row < sites.Length; row++)
                for (int column = 0; column < Sides.Length; column++)
                {
                    var focus = new Vector3(sites[row].x, 2, sites[row].z);
                    float yaw = sites[row].yaw + 180 + Sides[column];
                    float distance = row == 0 ? 42 : 24 + column * 12;
                    camera.Shoot(focus, yaw, 28, distance, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
                    camera.Shoot(focus, yaw, 28, distance, new RectInt(column * tileW, (sites.Length - 1 - row) * tileH, tileW, tileH));
                }
            RenderSettings.fog = true;
            camera.Save(ShowroomStage.SheetPath("muat-bongkar"));
        }
    }
}
