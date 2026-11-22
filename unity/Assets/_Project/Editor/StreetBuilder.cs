using UnityEngine;
using Steel.Editor.Toon;

namespace Steel.Editor
{
    internal static class StreetBuilder
    {
        /// <summary>Height of the pavement's walking surface; the road lies about 10 cm below it.</summary>
        internal const float PavementTop = .155f;
        /// <summary>Where the east kerb opens for the dealer, along the main road.</summary>
        internal const float DrivewayZ = 3, DrivewayWidth = 4;
        /// <summary>The cross street cuts through both pavements between these lines.</summary>
        private const float CrossNorth = -7, CrossSouth = -17, PavementNorth = 55, PavementSouth = -35;

        internal static void Create()
        {
            var asphalt = CityMaterials.Surface("asphalt_02", new Vector2(6, 24));
            var concrete = CityMaterials.Surface("concrete_floor_02", new Vector2(3, 2));
            var line = CityMaterials.Plain("Marka jalan", new Color(.9f, .72f, .28f), 0, .22f);
            CityGeometry.Box("Jalan utama", new Vector3(0, -.04f, 5), new Vector3(14, .2f, 100), asphalt);
            CityGeometry.Box("Jalan silang", new Vector3(0, -.03f, -12), new Vector3(85, .2f, 10), asphalt);
            Material[] kerb = { CityMaterials.Plain("Kanstin putih", ToonPalette.Cream), CityMaterials.Plain("Kanstin hitam", ToonPalette.Ink) };
            foreach (int side in new[] { -1, 1 })
            {
                Pavement(side, CrossNorth, PavementNorth, concrete);
                Pavement(side, PavementSouth, CrossSouth, concrete);
                // Painted kerbstones, black and white in turn. They are a lip on the pavement, never a wall.
                for (int z = -32, stone = 0; z < 50; z += 2, stone++)
                {
                    if (side > 0 && Mathf.Abs(z - DrivewayZ) < DrivewayWidth / 2) continue;   // the dealer's way out
                    if (z > CrossSouth - 1 && z < CrossNorth + 1) continue;   // the cross street stays level
                    CityGeometry.Box("Kanstin", new Vector3(side * 7.05f, PavementTop - .09f, z), new Vector3(.2f, .2f, 1.9f), kerb[stone % 2], false);
                }
            }
            Markings(line, concrete);
        }

        private static void Pavement(int side, float south, float north, Material concrete) =>
            CityGeometry.Box("Trotoar", new Vector3(side * 9, PavementTop - .125f, (south + north) / 2),
                new Vector3(4, .25f, north - south), concrete);

        private static void Markings(Material line, Material concrete)
        {
            for (int z = -38; z < 54; z += 6)
                CityGeometry.Box("Marka tengah", new Vector3(0, .07f, z), new Vector3(.12f, .015f, 3), line, false);
            for (int x = -5; x <= 5; x += 2)
                CityGeometry.Box("Penyeberangan", new Vector3(x, .08f, -4), new Vector3(1, .015f, 3), concrete, false);
        }
    }
}
