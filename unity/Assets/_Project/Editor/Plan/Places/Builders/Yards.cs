using UnityEngine;
using Steel.Editor.Plan.Town;

namespace Steel.Editor.Plan.Places.Builders
{
    /// <summary>Open yards behind seng fences: truck and container depots, and dumps heaped with scrap.</summary>
    internal static class Yards
    {
        private static readonly Tone[] Boxes = { Tone.Orange, Tone.Teal, Tone.Navy, Tone.Brick, Tone.Yellow };
        internal const float Gate = 6;   // wide enough for a trailer to swing in

        internal static void Depot(TownKit kit, Lot lot)
        {
            Fenced(kit, lot, Tone.Paving);
            Hut(kit, lot);
            int rows = Mathf.Max(1, (int)((lot.Width - 8) / 3.2f));
            for (int row = 0; row < rows; row++)
                for (int level = 0; level <= lot.Pick(10 + row, 3) && lot.Depth > 10; level++)
                    Shapes.Box(level == 0 ? kit.Solid : kit.Trim, lot, lot.Pick(20 + row * 3 + level, Boxes),
                        new Vector3(-lot.Width / 2 + 2.4f + row * 3.2f, level * 2.6f, -lot.Depth / 2 + 4.2f), new Vector3(2.5f, 2.6f, 6.1f), Tone.Slate);
        }

        internal static void Dump(TownKit kit, Lot lot)
        {
            Fenced(kit, lot, Tone.Dirt);
            Hut(kit, lot);
            int heaps = 2 + (int)(lot.Width * lot.Depth / 90);
            for (int heap = 0; heap < heaps; heap++)
            {
                float reach = 1.4f + lot.Roll(30 + heap) * Mathf.Min(lot.Width, lot.Depth) * .14f;
                Vector3 spot = lot.Point((lot.Roll(40 + heap) - .5f) * (lot.Width - reach * 2 - 2), Lot.Foot, (lot.Roll(50 + heap) - .65f) * (lot.Depth - reach * 2 - 3));
                Props.Heap(kit, spot, reach, lot.Pick(60 + heap, 3));
            }
        }

        /// <summary>Ground, a fence on three sides, and gate posts on the street side.</summary>
        internal static void Fenced(TownKit kit, Lot lot, Tone ground, Tone fence = Tone.Seng)
        {
            float w = lot.Width, d = lot.Depth, post = (w - Gate) / 2;
            Shapes.Flat(kit.Trim, lot, ground, 0, 0, w, d, Lot.Floor);
            Shapes.Box(kit.Solid, lot, fence, new Vector3(0, 0, -d / 2 + .1f), new Vector3(w, 2.2f, .14f), Tone.Slate);
            foreach (int side in new[] { -1, 1 })
            {
                Shapes.Box(kit.Solid, lot, fence, new Vector3(side * (w / 2 - .1f), 0, 0), new Vector3(.14f, 2.2f, d), Tone.Slate);
                if (post > .6f)
                    Shapes.Box(kit.Solid, lot, fence, new Vector3(side * (Gate / 2 + post / 2), 0, d / 2 - .1f), new Vector3(post, 2.2f, .14f), Tone.Slate);
                Shapes.Box(kit.Solid, lot, Tone.Yellow, new Vector3(side * Gate / 2, 0, d / 2 - .1f), new Vector3(.35f, 2.8f, .35f), Tone.Ink);
            }
        }

        /// <summary>The yard office in the back corner.</summary>
        internal static void Hut(TownKit kit, Lot lot)
        {
            if (lot.Width < 12) return;
            var centre = new Vector3(lot.Width / 2 - 3, 0, -lot.Depth / 2 + 2.4f);
            Wall[] walls = Shapes.Box(kit.Solid, lot, Tone.Cream, centre, new Vector3(4.4f, 2.8f, 3.2f), Tone.Slate);
            walls[0].Panel(kit.Trim, Tone.Door, .4f, 0, 1.3f, 2.1f);
            walls[0].Panel(kit.Trim, Tone.Glass, 1.9f, 1, 3.9f, 2.1f);
            walls[0].Awning(kit.Trim, Tone.Orange, 0, 4.4f, 2.5f, 1.1f, .25f);
        }
    }
}
