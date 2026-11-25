using UnityEngine;
using Steel.Editor.Plan.Town;

namespace Steel.Editor.Plan.Places.Builders
{
    /// <summary>Houses: kampung homes each in its own paint, and estate homes that share one roof and one fence colour.</summary>
    internal static class Houses
    {
        internal static void Kampung(TownKit kit, Lot lot) =>
            Home(kit, lot, lot.Pick(2, Tones.Walls), lot.Pick(3, Tones.Roofs), lot.Pick(4, Tones.Accents), lot.Roll(5) < .2f);

        internal static void Estate(TownKit kit, Lot lot) => Home(kit, lot, lot.Pick(2, Tones.Walls), Tone.Terracotta, Tone.Teal, false);

        private static void Home(TownKit kit, Lot lot, Tone wall, Tone roof, Tone accent, bool upstairs)
        {
            float w = lot.Width - 1.4f, d = lot.Depth - 3, back = -.9f, height = upstairs ? 5.8f : 3;
            Shapes.Flat(kit.Trim, lot, Tone.Paving, 0, lot.Depth / 2 - 1.2f, lot.Width, 2.4f, Lot.Floor);
            Wall[] walls = Shapes.Box(kit.Solid, lot, wall, new Vector3(0, 0, back), new Vector3(w, height, d));
            bool hipped = lot.Roll(6) < .5f;
            Shapes.Roof(kit.Trim, lot, roof, wall, new Vector3(0, height, back), w + .9f, d + .9f, 1.2f + w * .1f, hipped ? d * .45f : 0);
            Wall front = walls[0];
            foreach (Wall face in walls) face.Panel(kit.Trim, accent, 0, 0, face.Width, .55f, .03f);
            float door = w * (lot.Roll(7) < .5f ? .3f : .7f);
            front.Panel(kit.Trim, Tone.Door, door - .45f, 0, door + .45f, 2.1f);
            float window = w - door;
            front.Panel(kit.Trim, Tone.White, window - .75f, .95f, window + .75f, 2.25f, .04f);
            front.Panel(kit.Trim, Tone.Glass, window - .62f, 1.08f, window + .62f, 2.12f, .07f);
            if (upstairs) front.Rows(kit.Trim, Tone.Glass, 2, 1, 3.8f, 0, 1.2f, .45f);
            walls[1].Rows(kit.Trim, Tone.Glass, 1, 1, 1.1f, 0, 1, .3f);
            walls[3].Rows(kit.Trim, Tone.Glass, 1, 1, 1.1f, 0, 1, .3f);
            front.Awning(kit.Trim, roof, w * .1f, w * .9f, 2.55f, 1.5f, .35f);
            // A low front fence with the gate left open in the middle.
            float run = (lot.Width - 2) / 2;
            foreach (int side in new[] { -1, 1 })
                Shapes.Box(kit.Solid, lot, accent, new Vector3(side * (lot.Width / 2 - run / 2 - .2f), 0, lot.Depth / 2 - .25f),
                    new Vector3(run - .4f, .85f, .12f), Tone.Cream);
            Shapes.Drum(kit.Trim, Tone.Leaf, lot.Point(-lot.Width / 2 + .9f, 0, lot.Depth / 2 - 1.1f), .55f, .3f, .8f, 6);
        }
    }
}
