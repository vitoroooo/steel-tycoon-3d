using UnityEngine;
using Steel.Editor.Plan.Town;

namespace Steel.Editor.Plan.Places.Builders
{
    /// <summary>A row of ruko: every unit its own colour, shutter, sign board and awning, over a paved apron.</summary>
    internal static class Shops
    {
        private const float Unit = 4.6f, Storey = 3.3f, Apron = 2.4f;

        internal static void Build(TownKit kit, Lot lot)
        {
            int units = Mathf.Max(1, Mathf.RoundToInt(lot.Width / Unit));
            float width = lot.Width / units, depth = lot.Depth - Apron;
            Shapes.Flat(kit.Trim, lot, Tone.Paving, 0, lot.Depth / 2 - Apron / 2, lot.Width, Apron, Lot.Floor);
            for (int i = 0; i < units; i++)
                Ruko(kit, lot.Part((i + .5f) * width - lot.Width / 2, -Apron / 2, width, depth, i + 1));
        }

        private static void Ruko(TownKit kit, Lot unit)
        {
            int floors = 2 + unit.Pick(1, 2);
            float w = unit.Width, height = floors * Storey + .5f;
            Tone wall = unit.Pick(2, Tones.Walls), accent = unit.Pick(3, Tones.Accents), shade = unit.Pick(4, Tones.Accents);
            Wall[] walls = Shapes.Box(kit.Solid, unit, wall, Vector3.zero, new Vector3(w, height, unit.Depth), Tone.Concrete);
            Wall front = walls[0];
            front.Panel(kit.Trim, unit.Roll(5) < .45f ? Tone.Seng : Tone.GlassDark, .35f, 0, w - .35f, 2.6f);
            front.Panel(kit.Trim, accent, .15f, 2.8f, w - .15f, 3.6f, .14f);
            front.Panel(kit.Trim, Tone.Cream, .5f, 3, w * .55f, 3.4f, .16f);   // the lettering block of the sign
            if (unit.Roll(6) < .7f) front.Awning(kit.Trim, shade, .1f, w - .1f, 2.75f, 1.5f);
            front.Rows(kit.Trim, Tone.Glass, 2, floors - 1, Storey + 1.1f, Storey, 1.4f);
            front.Panel(kit.Trim, accent, 0, height - .3f, w, height + .35f, .1f);
            walls[2].Rows(kit.Trim, Tone.GlassDark, 2, floors, 1.3f, Storey, 1, .35f);
            if (unit.Roll(7) < .45f)
                Shapes.Drum(kit.Trim, Tone.Orange, unit.Point(0, height, -unit.Depth * .25f), .55f, .55f, 1.1f);
        }
    }
}
