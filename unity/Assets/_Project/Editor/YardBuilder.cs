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
            CityGeometry.Box("Atap bengkel sortir", h + new Vector3(-4, 3.4f, -2), new Vector3(7, .2f, 7), rust);
            for (int z = -5; z <= 1; z += 6)
                CityGeometry.Box("Kolom lapak", h + new Vector3(-.5f, 1.7f, z), new Vector3(.13f, 3.4f, .13f), dark);
            CityGeometry.Box("Meja sortir", h + new Vector3(-3, .7f, -2), new Vector3(3, 1.4f, 1.2f), rust);
            CityGeometry.Box("Papan lapak", h + new Vector3(-3, 3.2f, -5.5f), new Vector3(7, 1, .2f), dark, false);
            CityGeometry.Sign("LAPAK ANDA  /  SORTIR", h + new Vector3(-3, 3.2f, -5.65f), .23f, new Color(1, .8f, .4f));
            var marker = CityGeometry.Shape("Area sortir", PrimitiveType.Cylinder, h + Vector3.up * .08f,
                new Vector3(3, .015f, 3), CityMaterials.Plain("Zona emas", new Color(.75f, .49f, .12f), .2f, .4f), false);
            CityGeometry.Sign("01  /  SORTIR DI SINI", h + Vector3.up * 1.4f, .12f, Color.white);
            Vector3 f = new Vector3(layout.Factory.X, 0, layout.Factory.Z);
            CityGeometry.Box("Pabrik / beton", f + new Vector3(9, 3.5f, 9), new Vector3(22, 7, 15), concrete);
            CityGeometry.Box("Pabrik / cladding", f + new Vector3(9, 7.5f, 9), new Vector3(22.4f, 1.2f, 15.4f), iron);
            CityGeometry.Box("Gerbang pabrik", f + new Vector3(7, 2.3f, 1.4f), new Vector3(6, 4.6f, .25f), rust);
            CityGeometry.Box("Kanopi penerimaan", f + new Vector3(1, 4, .4f), new Vector3(8, .18f, 6), iron);
            CityGeometry.Box("Papan pabrik", f + new Vector3(9, 5.8f, 1.3f), new Vector3(20, 1.5f, .3f), dark, false);
            CityGeometry.Sign("BAJA NUSANTARA  /  PABRIK AI", f + new Vector3(9, 5.8f, 1.08f), .34f, new Color(1, .81f, .48f));
            CityGeometry.Box("Timbangan jual", f + new Vector3(0, .1f, 0), new Vector3(3, .2f, 3), rust, false);
            CityGeometry.Sign("02  /  PENERIMAAN RONGSOK", f + new Vector3(0, 2.8f, 0), .15f, Color.white);
            for (int i = 0; i < 3; i++)
            {
                CityGeometry.Shape("Tangki baja", PrimitiveType.Cylinder, f + new Vector3(5 + i * 4, 10, 10),
                    new Vector3(2.3f, 2.6f, 2.3f), rust);
                CityGeometry.Shape("Pipa industri", PrimitiveType.Cylinder, f + new Vector3(5 + i * 4, 13, 10),
                    new Vector3(.35f, 1.5f, .35f), dark);
            }
        }
    }
}
