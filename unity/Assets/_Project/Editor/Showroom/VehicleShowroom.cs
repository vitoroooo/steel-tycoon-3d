using UnityEditor;
using UnityEngine;
using Steel.Editor.Models;
using Steel.Editor.Vehicles;

namespace Steel.Editor.Showroom
{
    /// <summary>Review sheets for vehicles: three-quarter views on top, detail close-ups below.</summary>
    public static class VehicleShowroom
    {
        [MenuItem("Steel/Showroom/Gerobak")]
        public static void Gerobak()
        {
            ShowroomStage.Create();
            AuthoredModel.Create("Vehicles/Gerobak", GerobakMaterials.For);
            var camera = new ShowroomCamera(ShowroomStage.Width, ShowroomStage.Height);
            int w = ShowroomStage.Width, h = ShowroomStage.Height;
            Vector3 middle = new Vector3(0, .55f, -.3f);
            camera.Shoot(middle, 35, 14, 4.7f, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            camera.Shoot(middle, 35, 14, 4.7f, new RectInt(0, h / 2, w / 2, h / 2));
            camera.Shoot(middle, 215, 16, 4.7f, new RectInt(w / 2, h / 2, w / 2, h / 2));
            camera.Shoot(middle, 90, 4, 5.4f, new RectInt(0, 0, w / 3, h / 2));
            camera.Shoot(new Vector3(.47f, .33f, 0), 62, 8, 2.2f, new RectInt(w / 3, 0, w / 3, h / 2));
            camera.Shoot(new Vector3(0, .75f, .5f), 150, 30, 3.2f, new RectInt(w / 3 * 2, 0, w / 3, h / 2));
            camera.Save(ShowroomStage.SheetPath("gerobak"));
        }
    }
}
