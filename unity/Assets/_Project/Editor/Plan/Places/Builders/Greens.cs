using UnityEngine;
using Steel.Editor.Plan.Town;

namespace Steel.Editor.Plan.Places.Builders
{
    /// <summary>Green and open land: parks, the football pitch, fields under crops, ponds and the hills at the edge of town.</summary>
    internal static class Greens
    {
        internal static void Park(TownKit kit, Lot lot)
        {
            Shapes.Flat(kit.Trim, lot, Tone.Grass, 0, 0, lot.Width, lot.Depth, Lot.Floor);
            Shapes.Flat(kit.Trim, lot, Tone.Paving, 0, 0, lot.Width, 1.6f, Lot.Floor + .02f);
            Shapes.Flat(kit.Trim, lot, Tone.Paving, 0, 0, 1.6f, lot.Depth, Lot.Floor + .02f);
            foreach (int x in new[] { -1, 1 })
                foreach (int z in new[] { -1, 1 })
                    Props.Tree(kit, lot.Part(x * lot.Width * .28f, z * lot.Depth * .28f, 2, 2, x * 2 + z + 5));
            Shapes.Box(kit.Solid, lot, Tone.Wood, new Vector3(1.6f, 0, 1.3f), new Vector3(1.8f, .45f, .5f), Tone.Wood);
        }

        internal static void Pitch(TownKit kit, Lot lot)
        {
            float w = lot.Width - 2, d = lot.Depth - 2;
            Shapes.Flat(kit.Trim, lot, Tone.Grass, 0, 0, lot.Width, lot.Depth, Lot.Floor);
            Shapes.Flat(kit.Trim, lot, Tone.White, 0, 0, w, d, Lot.Floor + .02f);
            Shapes.Flat(kit.Trim, lot, Tone.Leaf, 0, 0, w - .3f, d - .3f, Lot.Floor + .04f);
            Shapes.Flat(kit.Trim, lot, Tone.White, 0, 0, .15f, d, Lot.Floor + .06f);
            foreach (int side in new[] { -1, 1 })
                Shapes.Box(kit.Solid, lot, Tone.White, new Vector3(side * (w / 2 - .3f), 0, 0), new Vector3(.12f, 2.2f, Mathf.Min(5, d * .4f)));
        }

        internal static void Field(TownKit kit, Lot lot)
        {
            Shapes.Flat(kit.Trim, lot, Tone.Dirt, 0, 0, lot.Width, lot.Depth, Lot.Floor);
            Shapes.Flat(kit.Trim, lot, Tone.Field, 0, 0, lot.Width - 1, lot.Depth - 1, Lot.Floor + .03f);
            for (float row = -lot.Depth / 2 + 1.2f; row < lot.Depth / 2 - 1; row += 1.3f)
                Shapes.Box(kit.Trim, lot, Tone.LeafDark, new Vector3(0, 0, row), new Vector3(lot.Width - 2, .4f, .5f), Tone.Leaf);
            if (lot.Roll(1) < .35f) Props.Saung(kit, lot.Part(lot.Width / 2 - 2.2f, lot.Depth / 2 - 2.2f, 3, 3));
        }

        internal static void Pond(TownKit kit, Lot lot)
        {
            Shapes.Flat(kit.Trim, lot, Tone.Grass, 0, 0, lot.Width, lot.Depth, Lot.Floor);
            Shapes.Flat(kit.Trim, lot, Tone.Rock, 0, 0, lot.Width - 2, lot.Depth - 2, Lot.Floor + .02f);
            Shapes.Flat(kit.Trim, lot, Tone.Water, 0, 0, lot.Width - 3.2f, lot.Depth - 3.2f, Lot.Floor + .05f);
            Shapes.Box(kit.Trim, lot, Tone.Wood, new Vector3(0, 0, lot.Depth / 2 - 3), new Vector3(1.4f, .3f, 4), Tone.Wood);
            Props.Saung(kit, lot.Part(-lot.Width / 2 + 1.6f, lot.Depth / 2 - 1.6f, 3, 3));
        }

        internal static void Mound(TownKit kit, Lot lot)
        {
            float reach = Mathf.Min(lot.Width, lot.Depth) / 2, height = reach * (.5f + lot.Roll(1) * .4f);
            Vector3 foot = lot.Point(0, Lot.Foot, 0);
            Shapes.Drum(kit.Solid, Tone.Grass, foot, reach, reach * .55f, height * .55f, 10);
            Shapes.Drum(kit.Solid, lot.Roll(2) < .5f ? Tone.Rock : Tone.Leaf, foot + Vector3.up * height * .55f, reach * .55f, reach * .12f, height * .45f, 10);
        }
    }
}
