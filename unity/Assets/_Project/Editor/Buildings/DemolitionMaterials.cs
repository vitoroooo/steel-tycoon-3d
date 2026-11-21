using UnityEngine;

namespace Steel.Editor.Buildings
{
    /// <summary>Surfaces for the demolition site's models: the wreck, the remains next door, the excavator and the salvage.</summary>
    internal static class DemolitionMaterials
    {
        private static readonly Vector2 Metres = Vector2.one;   // UVs are authored in metres

        internal static Material Paint(string slot)
        {
            switch (slot)
            {
                case "concrete": return CityMaterials.Surface("cracked_concrete_wall", Metres, "Bongkaran beton", new Color(.62f, .6f, .57f));
                case "floor": return CityMaterials.Surface("concrete_floor_02", Metres, "Bongkaran pelat", new Color(.7f, .69f, .66f));
                case "brick": return CityMaterials.Surface("broken_brick_wall", Metres, "Bongkaran bata");
                case "plaster": return CityMaterials.Surface("beige_wall_001", Metres, "Bongkaran plester", new Color(.74f, .7f, .6f));
                case "rubble": return CityMaterials.Surface("concrete_debris", Metres, "Bongkaran puing", new Color(.6f, .58f, .55f));
                case "wood": return CityMaterials.Surface("weathered_planks", Metres, "Bongkaran papan");
                case "rebar": return CityMaterials.Plain("Besi beton", new Color(.36f, .2f, .14f), .5f, .2f);
                case "bamboo": return CityMaterials.Plain("Bambu", new Color(.6f, .5f, .32f), 0, .4f);
                case "paint_yellow": return CityMaterials.Plain("Ekskavator kuning", new Color(.85f, .55f, .06f), .25f, .45f);
                case "track": return CityMaterials.Surface("rusty_metal_04", Metres, "Ekskavator rantai", new Color(.42f, .38f, .35f));
                case "chrome": return CityMaterials.Plain("Ekskavator krom", new Color(.78f, .8f, .82f), 1, .85f);
                case "hose": return CityMaterials.Plain("Ekskavator selang", new Color(.04f, .04f, .04f), 0, .35f);
                case "paint_green": return CityMaterials.Surface("green_metal_rust", Metres, "Pintu bekas hijau", new Color(.8f, .9f, .8f));
                default: return SmelterMaterials.Paint(slot);   // steel_dark, glass, lamp, beacon, seng, paint_blue
            }
        }
    }
}
