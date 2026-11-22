using UnityEngine;
using Steel.Editor.Plan.Town;

namespace Steel.Editor.Plan.Places.Builders
{
    /// <summary>Small things that fill what the lots leave over, so no corner of the city stands bare.</summary>
    internal static class Props
    {
        private static readonly Tone[] Junk = { Tone.Rust, Tone.Slate, Tone.Seng }, Paint = { Tone.Orange, Tone.Teal, Tone.Yellow, Tone.Navy };

        internal static void Tree(TownKit kit, Lot lot)
        {
            float size = .75f + lot.Roll(1) * .6f;
            Vector3 foot = lot.Point(0, Lot.Foot, 0), crown = foot + Vector3.up * (1.9f * size + .2f);
            Tone leaf = lot.Roll(2) < .5f ? Tone.Leaf : Tone.LeafDark;
            Shapes.Drum(kit.Solid, Tone.Trunk, foot, .2f * size, .15f * size, 2.1f * size, 5);
            Shapes.Drum(kit.Trim, leaf, crown, 1.1f * size, 1.6f * size, .9f * size, 7);
            Shapes.Drum(kit.Trim, leaf, crown + Vector3.up * .9f * size, 1.6f * size, .5f * size, 1.5f * size, 7);
        }

        internal static void Garden(TownKit kit, Lot lot)
        {
            if (lot.Roll(3) < .4f) { Tree(kit, lot); return; }
            Shapes.Drum(kit.Trim, lot.Roll(2) < .5f ? Tone.Leaf : Tone.LeafDark, lot.Point(0, Lot.Foot, 0), .7f, .35f, .9f, 6);
            Shapes.Drum(kit.Solid, Tone.Terracotta, lot.Point(.8f, Lot.Foot, .2f), .22f, .28f, .55f, 6);
        }

        internal static void Stall(TownKit kit, Lot lot)
        {
            if (lot.Roll(3) < .5f) { Tree(kit, lot); return; }
            Wall[] walls = Shapes.Box(kit.Solid, lot, Tone.Wood, Vector3.zero, new Vector3(1.4f, 1, .9f), Tone.Cream);
            walls[0].Awning(kit.Trim, lot.Pick(4, Paint), -.2f, 1.6f, 2.1f, 1.1f, .2f);
        }

        internal static void Crate(TownKit kit, Lot lot)
        {
            if (lot.Roll(3) < .35f) { Tree(kit, lot); return; }
            Shapes.Box(kit.Solid, lot, lot.Pick(4, Paint), Vector3.zero, new Vector3(1.2f, .9f + lot.Roll(5) * .8f, 1.2f), Tone.Wood);
        }

        internal static void Scrap(TownKit kit, Lot lot)
        {
            if (lot.Roll(3) < .25f) { Tree(kit, lot); return; }
            if (lot.Roll(3) < .6f) Heap(kit, lot.Point(0, Lot.Foot, 0), .8f, lot.Pick(4, 3));
            else Shapes.Drum(kit.Solid, lot.Pick(4, Paint), lot.Point(0, Lot.Foot, 0), .32f, .32f, 1.1f);
        }

        /// <summary>A pile of scrap: a blunt cone with a smaller one leaning on it.</summary>
        internal static void Heap(TownKit kit, Vector3 foot, float reach, int kind)
        {
            Shapes.Drum(kit.Solid, Junk[kind % Junk.Length], foot, reach, reach * .3f, reach * .75f, 7);
            Shapes.Drum(kit.Trim, Junk[(kind + 1) % Junk.Length], foot + new Vector3(reach * .5f, 0, reach * .3f), reach * .55f, reach * .1f, reach * .5f, 6);
        }

        /// <summary>A field hut: four posts under a thatch-coloured roof.</summary>
        internal static void Saung(TownKit kit, Lot lot)
        {
            foreach (int x in new[] { -1, 1 })
                foreach (int z in new[] { -1, 1 })
                    Shapes.Drum(kit.Solid, Tone.Wood, lot.Point(x * 1.1f, Lot.Foot, z * 1.1f), .08f, .08f, 2.3f, 4);
            Shapes.Flat(kit.Trim, lot, Tone.Wood, 0, 0, 2.4f, 2.4f, .5f);
            Shapes.Roof(kit.Trim, lot, Tone.Butter, Tone.Wood, new Vector3(0, 2, 0), 3.2f, 3.2f, 1.1f, 1.2f);
        }

        /// <summary>A name board on two posts, facing the street.</summary>
        internal static void Board(TownKit kit, Lot lot, string text)
        {
            float w = lot.Width - .6f;
            foreach (int side in new[] { -1, 1 })
                Shapes.Drum(kit.Solid, Tone.Slate, lot.Point(side * (w / 2 - .1f), Lot.Foot, 0), .08f, .08f, 3.4f, 5);
            Wall[] faces = Shapes.Box(kit.Trim, lot, Tone.Navy, new Vector3(0, 2.1f, 0), new Vector3(w, 1.3f, .14f), Tone.Yellow);
            faces[0].Panel(kit.Trim, Tone.Yellow, 0, 0, w, .12f, .02f);
            CityGeometry.Sign(text, lot.Point(0, 2.8f, .1f), text.Length > 18 ? .26f : .34f, Tones.Colour(Tone.Cream), lot.Yaw + 180);
        }
    }
}
