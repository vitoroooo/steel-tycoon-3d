using System;
using UnityEngine;

namespace Steel.Editor.Buildings
{
    /// <summary>Surfaces for what stands in yards: the seng fence panel and the banana plant.</summary>
    internal static class YardMaterials
    {
        private static readonly Vector2 Metres = Vector2.one;   // UVs are authored in metres

        internal static Material Fence(string slot)
        {
            switch (slot)
            {
                case "seng": return CityMaterials.Surface("corrugated_iron", Metres, "Pagar seng", new Color(.8f, .8f, .78f));
                case "seng_rust": return CityMaterials.Surface("rusty_metal_04", Metres, "Pagar seng karat");
                case "wood": return CityMaterials.Surface("weathered_planks", Metres, "Pagar kayu", new Color(.55f, .45f, .36f));
                default: throw new ArgumentException("Unknown fence material slot: " + slot);
            }
        }

        internal static Material Banana(string slot)
        {
            switch (slot)
            {
                case "leaf": return FoliageMaterial.Cutout("Daun pisang", "banana_leaf", "banana_leaf_normal");
                case "leaf_dry": return FoliageMaterial.Cutout("Daun pisang kering", "banana_leaf_dry", "banana_leaf_normal");
                case "stem": return FoliageMaterial.Opaque("Batang pisang", "banana_stem", Metres, .25f);
                case "banana": return CityMaterials.Plain("Buah pisang", new Color(.42f, .56f, .16f), 0, .4f);
                case "heart": return CityMaterials.Plain("Jantung pisang", new Color(.4f, .12f, .22f), 0, .5f);
                default: throw new ArgumentException("Unknown banana material slot: " + slot);
            }
        }
    }
}
