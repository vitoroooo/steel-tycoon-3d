using UnityEngine;
using Steel.Services;
using Steel.Presentation.World;

namespace Steel.Editor
{
    internal static class YardBuilder
    {
        internal static void Create(CityLayout layout)
        {
            var iron = CityMaterials.Surface("corrugated_iron", new Vector2(4, 2));
            var rust = CityMaterials.Surface("rusty_metal_04", new Vector2(2, 1));
            var concrete = CityMaterials.Surface("concrete_floor_02", new Vector2(3, 2));
            var dark = CityMaterials.Plain("Baja gelap", new Color(.09f, .12f, .14f), .7f, .45f);
            Vector3 h = new Vector3(layout.Home.X, 0, layout.Home.Z);
            CityGeometry.Box("Lapak / lantai", h + Vector3.down * .02f, new Vector3(15, .15f, 12), concrete);
            CityGeometry.Box("Pagar belakang lapak", h + new Vector3(0, 1.5f, -6), new Vector3(15, 3, .2f), iron);
            CityGeometry.Box("Dinding lapak", h + new Vector3(-7.5f, 1.5f, 0), new Vector3(.2f, 3, 12), iron);
            RoofBuilder.Create(h + new Vector3(-4, 3.4f, -2), 7.5f, 7.5f, 1.1f);
            for (int z = -5; z <= 1; z += 6)
                CityGeometry.Box("Kolom lapak", h + new Vector3(-.5f, 1.7f, z), new Vector3(.13f, 3.4f, .13f), dark);
            CityGeometry.Box("Meja sortir", h + new Vector3(-3, .7f, -2), new Vector3(3, 1.4f, 1.2f), rust);
            CityGeometry.Box("Papan lapak", h + new Vector3(-3, 3.2f, -5.5f), new Vector3(7, 1, .2f), dark, false);
            CityGeometry.Sign("LAPAK ANDA  /  SORTIR", h + new Vector3(-3, 3.2f, -5.65f), .23f, new Color(1, .8f, .4f));
            var marker = CityGeometry.Shape("Area sortir", PrimitiveType.Cylinder, h + Vector3.up * .08f,
                new Vector3(3, .015f, 3), CityMaterials.Plain("Zona emas", new Color(.75f, .49f, .12f), .2f, .4f), false);
        }
    }
}
