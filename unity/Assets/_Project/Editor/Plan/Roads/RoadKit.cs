using UnityEngine;
using Steel.Editor.Toon;

namespace Steel.Editor.Plan.Roads
{
    /// <summary>One strip per street material, shared by every street so the whole network is a few meshes per sector.</summary>
    internal sealed class RoadKit
    {
        internal readonly Strip Asphalt = new Strip(), Walk = new Strip(), PaleKerb = new Strip(), DarkKerb = new Strip();
        internal readonly Strip Yellow = new Strip(), White = new Strip(), Grass = new Strip(), Concrete = new Strip();

        internal void Build()
        {
            var concrete = CityMaterials.Surface("concrete_floor_02", Vector2.one);
            Asphalt.Build("Aspal", CityMaterials.Surface("asphalt_02", Vector2.one), true);
            Walk.Build("Trotoar", concrete, true);
            Concrete.Build("Beton jalan", concrete, true);
            PaleKerb.Build("Kanstin putih", CityMaterials.Plain("Kanstin putih", ToonPalette.Cream), false);
            DarkKerb.Build("Kanstin hitam", CityMaterials.Plain("Kanstin hitam", ToonPalette.Ink), false);
            Yellow.Build("Marka kuning", CityMaterials.Plain("Marka jalan", ToonPalette.Yellow), false);
            White.Build("Marka putih", CityMaterials.Plain("Marka putih", ToonPalette.Cream), false);
            Grass.Build("Rumput jalan", CityMaterials.Plain("Rumput median", ToonPalette.Hex("7DB048")), false);
        }
    }
}
