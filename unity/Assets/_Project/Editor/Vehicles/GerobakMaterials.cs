using UnityEngine;

namespace Steel.Editor.Vehicles
{
    /// <summary>Maps the material slots of Gerobak.fbx to surfaces: chipped paint, rust, old wood.</summary>
    internal static class GerobakMaterials
    {
        private static readonly Vector2 Metres = Vector2.one;   // UVs are authored in metres

        private static readonly Color Frame = new Color(.62f, .6f, .58f);   // dulls the red paint

        internal static Material For(string slot)
        {
            switch (slot)
            {
                case "frame": return CityMaterials.Surface("rusty_painted_metal", Metres, "Gerobak rangka", Frame);
                case "planks": return CityMaterials.Surface("blue_painted_planks", Metres, "Gerobak papan cat");
                case "wood": return CityMaterials.Surface("weathered_planks", Metres, "Gerobak papan tua");
                case "signboard": return CityMaterials.Surface("weathered_planks", Metres, "Gerobak papan tua");
                case "steel": return CityMaterials.Surface("rusty_metal_04", Metres, "Gerobak besi");
                case "sack": return CityMaterials.Surface("hessian_230", Metres, "Gerobak karung");
                case "rubber": return CityMaterials.Plain("Gerobak karet", new Color(.035f, .035f, .04f), 0, .28f);
                case "spoke": return CityMaterials.Plain("Gerobak jari-jari", new Color(.55f, .55f, .56f), .9f, .55f);
                case "brass": return CityMaterials.Plain("Gerobak kuningan", new Color(.62f, .47f, .2f), .85f, .5f);
                case "rope": return CityMaterials.Plain("Gerobak tali", new Color(.55f, .44f, .27f), 0, .15f);
                case "paint": return CityMaterials.Plain("Gerobak cat putih", new Color(.88f, .85f, .76f), 0, .3f);
                default: throw new System.ArgumentException("Unknown gerobak material slot: " + slot);
            }
        }
    }
}
