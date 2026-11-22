using System.Collections.Generic;
using UnityEngine;

namespace Steel.Editor.Toon
{
    /// <summary>Repaints downloaded prop models in flat palette colours, so they sit in the same picture as the city.</summary>
    internal static class ModelPaint
    {
        private static readonly Dictionary<string, string> Colours = new Dictionary<string, string>
        {
            { "barrel_03", "3F7FB5" }, { "old_tyre", "2B2E38" }, { "wooden_crate_01", "B07B4A" },
            { "modular_industrial_pipes_01", "9A5230" }, { "street_lamp_01", "1F2F4A" },
            { "modular_electric_cables", "1A1C24" },
        };

        /// <summary>Models without an entry keep their own materials (see-through fences, the old pickup).</summary>
        internal static void Apply(GameObject model, string name)
        {
            if (!Colours.TryGetValue(name, out string code)) return;
            Material paint = CityMaterials.Outlined("Kartun " + name, ToonPalette.Hex(code));
            foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
            {
                var slots = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < slots.Length; i++) slots[i] = paint;
                renderer.sharedMaterials = slots;
            }
        }
    }
}
