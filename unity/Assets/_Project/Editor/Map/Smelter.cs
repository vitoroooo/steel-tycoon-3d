using UnityEngine;
using Steel.Editor.Buildings;
using Steel.Editor.Effects;
using Steel.Editor.Models;
using Steel.Presentation.World;

namespace Steel.Editor.Map
{
    /// <summary>The melt shop of PT Baja Nusantara: crane hall, arc furnace, stacks, smoke, sparks and furnace light.</summary>
    internal static class Smelter
    {
        internal static readonly Vector3 Hall = new Vector3(43, 0, -50);
        private const float Facing = -90;   // the open working side looks west, across the scrap yard to the main road

        internal static void Create()
        {
            var hall = Solid.Stand(AuthoredModel.Create("Buildings/Peleburan", SmelterMaterials.Paint), Hall, Facing);
            Solid.Box(hall, new Vector3(0, 5.5f, -9), new Vector3(30, 11, .4f));
            Solid.Box(hall, new Vector3(-15, 5.5f, 0), new Vector3(.4f, 11, 18));
            foreach (int side in new[] { -1, 1 })
            {
                Solid.Box(hall, new Vector3(side * 12, 5.5f, 9), new Vector3(6, 11, .4f));
                Solid.Box(hall, new Vector3(15, 5.5f, side * 6), new Vector3(.4f, 11, 6));
            }
            CityGeometry.Sign("PT BAJA NUSANTARA", hall.transform.TransformPoint(0, 8.85f, 9.3f), .46f, new Color(1, .81f, .48f), 90);
            CityGeometry.Sign("PELEBURAN BESI  /  PABRIK AI", hall.transform.TransformPoint(0, 8.15f, 9.3f), .2f, Color.white, 90);
            Furnace(hall.transform);
            foreach (float along in new[] { -6.2f, -4.6f })   // bars fresh off the caster, still glowing
            {
                var hot = AuthoredModel.Create("Props/Billet", SmelterMaterials.HotBillet);
                Solid.Stand(hot, hall.transform.TransformPoint(9.5f, .05f, along), Facing + 90);
                Solid.Box(hot, Vector3.up * .5f, new Vector3(1.1f, 1, 6));
            }
            Glow("Pijar billet", hall.transform.TransformPoint(9.5f, 1.8f, -5.4f), 9, 3.5f);
            Stack(new Vector3(47, 0, -29), 1, new Color(.86f, .86f, .84f, .55f));
            Stack(new Vector3(39, 0, -30), .75f, new Color(.42f, .4f, .39f, .5f));
            SmokeBuilder.Create("Uap atap peleburan", hall.transform.TransformPoint(-4, 15.2f, 0), 2.2f, new Color(.8f, .78f, .74f, .22f), 9);
        }

        private static void Furnace(Transform hall)
        {
            var furnace = Solid.Stand(AuthoredModel.Create("Props/Tungku", SmelterMaterials.Paint), hall.TransformPoint(-4, .05f, -1), Facing);
            Solid.Box(furnace, new Vector3(0, 2.5f, 0), new Vector3(5, 5, 5));
            Solid.Box(furnace, new Vector3(0, 4, -4.5f), new Vector3(2, 8, 1));
            Solid.Box(furnace, new Vector3(5.6f, 1.5f, 0), new Vector3(3.2f, 3, 2.6f));
            Solid.Box(furnace, new Vector3(-.5f, 1.2f, -4.6f), new Vector3(9, 2.4f, 3.4f));
            SparkBuilder.Create(furnace.transform.TransformPoint(4.3f, 3.1f, 0));
            Glow("Pijar tungku", furnace.transform.TransformPoint(1.5f, 4.6f, 2.2f), 20, 10);
            Glow("Pijar ladle", furnace.transform.TransformPoint(5.6f, 4, .5f), 11, 5);
        }

        private static void Stack(Vector3 at, float scale, Color smoke)
        {
            var stack = Solid.Stand(AuthoredModel.Create("Props/Cerobong", SmelterMaterials.Paint), at, 0, scale);
            Solid.Box(stack, new Vector3(0, 8, 0), new Vector3(3.4f, 16, 3.4f));
            SmokeBuilder.Create("Asap cerobong", at + Vector3.up * 31.6f * scale, 1.9f * scale, smoke);
        }

        private static void Glow(string name, Vector3 at, float range, float intensity)
        {
            var go = new GameObject(name); go.transform.SetParent(CityGeometry.Root, false); go.transform.position = at;
            var light = go.AddComponent<Light>(); light.type = LightType.Point; light.range = range;
            light.color = new Color(1, .5f, .16f); light.intensity = intensity; light.shadows = LightShadows.None;
            go.AddComponent<FurnaceGlow>().Intensity = intensity;
        }
    }
}
