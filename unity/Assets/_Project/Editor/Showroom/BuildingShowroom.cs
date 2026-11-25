using UnityEditor;
using UnityEngine;
using Steel.Editor.Buildings;
using Steel.Editor.Models;

namespace Steel.Editor.Showroom
{
    /// <summary>Review sheets for buildings: the street face, the alley face, then close-ups.</summary>
    public static class BuildingShowroom
    {
        private const float UnitWidth = 5;

        [MenuItem("Steel/Showroom/Ruko")]
        public static void Ruko()
        {
            ShowroomStage.Create();
            string[] models = { "Buildings/Ruko", "Buildings/Ruko3", "Buildings/Ruko" };
            for (int i = 0; i < models.Length; i++)   // a short terrace, as rukos are always built in rows
                AuthoredModel.Create(models[i], RukoMaterials.Paint(i + 1)).transform.SetPositionAndRotation(
                    Vector3.right * (i - 1) * UnitWidth, Quaternion.Euler(0, 180, 0));   // street face to the key light
            int w = ShowroomStage.Width, h = ShowroomStage.Height;
            var camera = new ShowroomCamera(w, h);
            Vector3 middle = new Vector3(0, 4, 0);
            camera.Shoot(middle, 205, 8, 34, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            camera.Shoot(middle, 205, 8, 34, new RectInt(0, h / 2, w / 2, h / 2));
            camera.Shoot(middle, 25, 10, 34, new RectInt(w / 2, h / 2, w / 2, h / 2));
            camera.Shoot(new Vector3(5, 1.8f, -4), 200, 4, 12, new RectInt(0, 0, w / 3, h / 2));
            camera.Shoot(new Vector3(0, 1.6f, 4.6f), 20, 6, 11, new RectInt(w / 3, 0, w / 3, h / 2));
            camera.Shoot(new Vector3(0, 9.5f, 1), 330, 28, 16, new RectInt(w / 3 * 2, 0, w / 3, h / 2));
            camera.Save(ShowroomStage.SheetPath("ruko"));
        }

        [MenuItem("Steel/Showroom/Rumah")]
        public static void Rumah()
        {
            ShowroomStage.Create();
            for (int i = 0; i < 2; i++)   // two neighbours, with the walking gap kampung houses leave between them
                AuthoredModel.Create("Buildings/Rumah", RumahMaterials.Paint(i)).transform.SetPositionAndRotation(
                    Vector3.right * (i * 7 - 3.5f), Quaternion.Euler(0, 180, 0));   // lane face to the key light
            int w = ShowroomStage.Width, h = ShowroomStage.Height;
            var camera = new ShowroomCamera(w, h);
            Vector3 middle = new Vector3(0, 2, 0);
            camera.Shoot(middle, 205, 8, 30, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            camera.Shoot(middle, 205, 8, 30, new RectInt(0, h / 2, w / 2, h / 2));
            camera.Shoot(middle, 25, 12, 30, new RectInt(w / 2, h / 2, w / 2, h / 2));
            camera.Shoot(new Vector3(3.5f, 1.4f, -3), 190, 5, 11, new RectInt(0, 0, w / 3, h / 2));
            camera.Shoot(new Vector3(3.5f, 1.2f, 4.5f), 15, 8, 12, new RectInt(w / 3, 0, w / 3, h / 2));
            camera.Shoot(new Vector3(0, 3.6f, 0), 240, 35, 20, new RectInt(w / 3 * 2, 0, w / 3, h / 2));
            camera.Save(ShowroomStage.SheetPath("rumah"));
        }
    }
}
