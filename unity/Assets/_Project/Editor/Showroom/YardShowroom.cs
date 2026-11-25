using UnityEditor;
using UnityEngine;
using Steel.Editor.Buildings;
using Steel.Editor.Map;
using Steel.Editor.Models;

namespace Steel.Editor.Showroom
{
    /// <summary>Design sheet for yard dressing: the banana plant in front of a run of seng fence.</summary>
    public static class YardShowroom
    {
        [MenuItem("Steel/Showroom/Halaman")]
        public static void Halaman()
        {
            ShowroomStage.Create();
            SengFence.Run(new Vector3(4.8f, 0, 1.5f), new Vector3(-4.8f, 0, 1.5f));
            for (int i = 0; i < 2; i++)
            {
                var plant = AuthoredModel.Create("Nature/Pisang", YardMaterials.Banana);
                plant.transform.SetPositionAndRotation(new Vector3(i * 3.4f - 1.7f, 0, -.2f), Quaternion.Euler(0, 200 + i * 130, 0));
                plant.transform.localScale = Vector3.one * (1 + i * .12f);
            }
            int w = ShowroomStage.Width, h = ShowroomStage.Height;
            var camera = new ShowroomCamera(w, h);
            Vector3 middle = new Vector3(0, 1.4f, .4f);
            camera.Shoot(middle, 200, 8, 13, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            camera.Shoot(middle, 200, 8, 13, new RectInt(0, 0, w / 2, h));
            camera.Shoot(new Vector3(-1.5f, 1.9f, -.2f), 150, 12, 5.5f, new RectInt(w / 2, h / 2, w / 2, h / 2));
            camera.Shoot(new Vector3(2.4f, 1, 1.5f), 215, 6, 5, new RectInt(w / 2, 0, w / 2, h / 2));
            camera.Save(ShowroomStage.SheetPath("halaman"));
        }
    }
}
