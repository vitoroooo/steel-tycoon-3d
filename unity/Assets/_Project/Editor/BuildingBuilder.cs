using UnityEngine;

namespace Steel.Editor
{
    internal static class BuildingBuilder
    {
        internal static void Block(float x, float z, int floors, string sign)
        {
            var wall = CityMaterials.Surface("concrete_floor_02", new Vector2(3, 2));
            var metal = CityMaterials.Surface("corrugated_iron", new Vector2(3, 1));
            var frame = CityMaterials.Plain("Baja gelap", new Color(.09f, .12f, .14f), .7f, .45f);
            var glass = CityMaterials.Plain("Kaca biru", new Color(.1f, .2f, .25f), .65f, .92f);
            var warm = CityMaterials.Glow("Kaca hangat", new Color(.65f, .36f, .12f));
            float height = floors * 3.1f;
            CityGeometry.Box(sign, new Vector3(x, height / 2, z), new Vector3(11, height, 9), wall);
            for (int f = 0; f < floors; f++)
            {
                float y = 1.8f + f * 3.1f;
                CityGeometry.Box("Lis lantai", new Vector3(x, f * 3.1f + .2f, z - 4.6f),
                    new Vector3(11.3f, .2f, .4f), wall);
                for (int j = -2; j <= 2; j++)
                {
                    Vector3 p = new Vector3(x + j * 2, y, z - 4.56f);
                    CityGeometry.Box("Bingkai jendela", p, new Vector3(1.5f, 1.9f, .18f), frame, false);
                    CityGeometry.Box("Kaca", p + Vector3.back * .11f, new Vector3(1.22f, 1.62f, .06f),
                        f == 1 && j % 2 == 0 ? warm : glass, false);
                    CityGeometry.Box("Palang", p + Vector3.back * .16f, new Vector3(.06f, 1.8f, .07f), wall, false);
                }
            }
            CityGeometry.Box("Parapet", new Vector3(x, height + .2f, z), new Vector3(11.4f, .4f, 9.4f), metal);
            CityGeometry.Box("AC atap", new Vector3(x + 2, height + .9f, z), new Vector3(2.5f, 1.3f, 1.8f), frame);
            CityGeometry.Box("Papan toko", new Vector3(x, 3, z - 4.8f), new Vector3(9.5f, 1, .22f), frame, false);
            CityGeometry.Sign(sign, new Vector3(x, 3, z - 4.94f), .23f, new Color(1, .8f, .46f));
        }
        internal static void Create()
        {
            Block(-25, 23, 3, "TOKO TEKNIK  /  MAJU JAYA");
            Block(-25, 40, 4, "GUDANG SENTOSA");
            Block(26, -7, 3, "BENGKEL DIESEL");
            Block(28, 43, 4, "NUSANTARA LOGISTIK");
        }
    }
}
