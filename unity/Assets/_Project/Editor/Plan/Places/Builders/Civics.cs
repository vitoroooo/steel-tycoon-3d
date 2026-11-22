using UnityEngine;
using Steel.Editor.Plan.Town;

namespace Steel.Editor.Plan.Places.Builders
{
    /// <summary>Public buildings: a wide hall under a hipped roof with a columned porch, and the mosque with dome and minaret.</summary>
    internal static class Civics
    {
        internal static void Hall(TownKit kit, Lot lot)
        {
            float w = lot.Width - 4, d = lot.Depth - 6;
            int floors = lot.Width > 20 ? 3 : 2;
            float height = floors * 3.6f + .6f;
            Tone accent = lot.Pick(3, Tones.Accents);
            Shapes.Flat(kit.Trim, lot, Tone.Paving, 0, 0, lot.Width, lot.Depth, Lot.Floor);
            Wall[] walls = Shapes.Box(kit.Solid, lot, Tone.White, new Vector3(0, 0, -1), new Vector3(w, height, d));
            Shapes.Roof(kit.Trim, lot, Tone.Terracotta, Tone.White, new Vector3(0, height, -1), w + 1.6f, d + 1.6f, 3, d * .5f);
            foreach (Wall face in walls)
            {
                face.Panel(kit.Trim, accent, 0, 0, face.Width, .9f, .04f);
                face.Rows(kit.Trim, Tone.Glass, Mathf.Max(1, (int)(face.Width / 3)), floors, 1.5f, 3.6f, 1.8f, .5f);
            }
            float porch = Mathf.Min(w * .4f, 10), front = d / 2 - 1;
            Shapes.Box(kit.Trim, lot, Tone.Cream, new Vector3(0, 4.4f, front + 1.6f), new Vector3(porch, .7f, 3.2f), Tone.Concrete);
            for (int column = 0; column < 4; column++)
                Shapes.Drum(kit.Solid, Tone.Cream, lot.Point(-porch / 2 + .4f + column * (porch - .8f) / 3, Lot.Foot, front + 2.8f), .3f, .3f, 4.6f);
            walls[0].Panel(kit.Trim, Tone.Door, w / 2 - 1.3f, 0, w / 2 + 1.3f, 3);
            Shapes.Drum(kit.Trim, Tone.Seng, lot.Point(-lot.Width / 2 + 2, 0, lot.Depth / 2 - 2), .06f, .06f, 8, 5);
            Shapes.Face(kit.Trim, Tone.Brick, lot.Point(-lot.Width / 2 + 2, 7.9f, lot.Depth / 2 - 2), lot.Point(-lot.Width / 2 + 3.5f, 7.9f, lot.Depth / 2 - 2),
                lot.Point(-lot.Width / 2 + 3.5f, 7.4f, lot.Depth / 2 - 2), lot.Point(-lot.Width / 2 + 2, 7.4f, lot.Depth / 2 - 2), lot.Direction(0, 1));
        }

        internal static void Mosque(TownKit kit, Lot lot)
        {
            float side = Mathf.Min(lot.Width, lot.Depth) - 6;
            Shapes.Flat(kit.Trim, lot, Tone.Paving, 0, 0, lot.Width, lot.Depth, Lot.Floor);
            Wall[] walls = Shapes.Box(kit.Solid, lot, Tone.White, Vector3.zero, new Vector3(side, 6, side), Tone.Concrete);
            foreach (Wall face in walls)
            {
                face.Panel(kit.Trim, Tone.Teal, 0, 0, face.Width, 1, .04f);
                face.Rows(kit.Trim, Tone.GlassDark, Mathf.Max(1, (int)(face.Width / 3)), 1, 1.6f, 0, 3, .4f);
            }
            walls[0].Awning(kit.Trim, Tone.Teal, side * .25f, side * .75f, 4, 2.4f, .3f);
            // The dome: three rings that close in, with a gold tip.
            Vector3 top = lot.Point(0, 6, 0);
            float r = side * .32f;
            Shapes.Drum(kit.Trim, Tone.Cream, top, r, r, 1.2f, 12);
            Shapes.Drum(kit.Trim, Tone.Teal, top + Vector3.up * 1.2f, r * 1.05f, r * .85f, r * .5f, 12);
            Shapes.Drum(kit.Trim, Tone.Teal, top + Vector3.up * (1.2f + r * .5f), r * .85f, 0, r * .6f, 12);
            Shapes.Drum(kit.Trim, Tone.Yellow, top + Vector3.up * (1.2f + r * 1.05f), .12f, 0, 1.4f, 5);
            Vector3 minaret = lot.Point(lot.Width / 2 - 2.5f, Lot.Foot, lot.Depth / 2 - 2.5f);
            Shapes.Drum(kit.Solid, Tone.White, minaret, 1, .8f, 13);
            Shapes.Drum(kit.Trim, Tone.Cream, minaret + Vector3.up * 13, 1.3f, 1.3f, .5f);
            Shapes.Drum(kit.Trim, Tone.Teal, minaret + Vector3.up * 13.5f, .9f, 0, 2.4f);
        }
    }
}
