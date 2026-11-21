using UnityEngine;

namespace Steel.Editor
{
    internal static class StreetBuilder
    {
        internal static void Create()
        {
            var asphalt = CityMaterials.Surface("asphalt_02", new Vector2(6, 24));
            var concrete = CityMaterials.Surface("concrete_floor_02", new Vector2(3, 2));
            var dirt = CityMaterials.Surface("brown_mud_02", new Vector2(20, 20));
            var line = CityMaterials.Plain("Marka jalan", new Color(.9f, .72f, .28f), 0, .22f);
            CityGeometry.Box("Latar kawasan", new Vector3(0, -.75f, 0), new Vector3(500, .4f, 500), dirt, false);
            CityGeometry.Box("Tanah kawasan", new Vector3(0, -.3f, 5), new Vector3(110, .4f, 120), dirt);
            CityGeometry.Box("Jalan utama", new Vector3(0, -.04f, 5), new Vector3(14, .2f, 100), asphalt);
            CityGeometry.Box("Jalan silang", new Vector3(0, -.03f, -12), new Vector3(85, .2f, 10), asphalt);
            foreach (int side in new[] { -1, 1 })
            {
                CityGeometry.Box("Trotoar", new Vector3(side * 9, .03f, 10), new Vector3(4, .25f, 90), concrete);
                for (int z = -32; z < 50; z += 2)
                    CityGeometry.Box("Kanstin", new Vector3(side * 7.05f, .14f, z), new Vector3(.2f, .3f, 1.9f), concrete);
            }
            for (int z = -38; z < 54; z += 6)
                CityGeometry.Box("Marka tengah", new Vector3(0, .07f, z), new Vector3(.12f, .015f, 3), line, false);
            for (int x = -5; x <= 5; x += 2)
                CityGeometry.Box("Penyeberangan", new Vector3(x, .08f, -4), new Vector3(1, .015f, 3), concrete, false);
        }
    }
}
