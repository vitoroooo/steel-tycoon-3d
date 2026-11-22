using UnityEngine;
using Steel.Editor.Plan.Town;

namespace Steel.Editor.Plan.Places.Builders.Landmarks
{
    /// <summary>A truck fuel station: pumps under a high canopy, a shop at the back and a price totem on the street.</summary>
    internal static class Fuel
    {
        private const float Clear = 5.4f, Shop = 4.5f;   // high enough for a tronton to drive under
        internal const string Board = "SPBU SUKABAJA";

        internal static void Build(TownKit kit, Lot lot)
        {
            float w = lot.Width, d = lot.Depth, canopy = Mathf.Max(6, w - 8), deep = Mathf.Max(5, d - Shop - 6), middle = d / 2 - 2 - deep / 2;
            Shapes.Flat(kit.Solid, lot, Tone.Concrete, 0, 0, w, d, Lot.Floor);   // solid: vehicles drive in
            Wall[] roof = Shapes.Box(kit.Trim, lot, Tone.White, new Vector3(0, Clear, middle), new Vector3(canopy, .9f, deep), Tone.Orange);
            foreach (Wall edge in roof) edge.Panel(kit.Trim, Tone.Orange, 0, .25f, edge.Width, .65f, .03f);
            CityGeometry.Sign(Board, lot.Point(0, Clear + .45f, middle + deep / 2 + .08f), .42f, Tones.Colour(Tone.White), lot.Yaw + 180);
            foreach (int side in new[] { -1, 1 })
                foreach (int end in new[] { -1, 1 })
                    Shapes.Drum(kit.Solid, Tone.White, lot.Point(side * (canopy / 2 - 1), Lot.Foot, middle + end * (deep / 2 - 1)), .28f, .28f, Clear - Lot.Foot, 8);
            // Pump islands across the canopy, each a kerb with two pumps.
            int islands = Mathf.Max(1, (int)(canopy / 7));
            for (int island = 0; island < islands; island++)
            {
                float x = (island - (islands - 1) / 2f) * 7;
                Shapes.Box(kit.Solid, lot, Tone.Yellow, new Vector3(x, 0, middle), new Vector3(1.2f, .2f, deep * .55f), Tone.Concrete);
                foreach (int end in new[] { -1, 1 })
                {
                    Wall[] pump = Shapes.Box(kit.Solid, lot, Tone.Orange, new Vector3(x, .2f, middle + end * deep * .15f), new Vector3(.8f, 1.6f, .55f), Tone.White);
                    pump[1].Panel(kit.Trim, Tone.GlassDark, .08f, .9f, .47f, 1.4f, .02f);
                    pump[3].Panel(kit.Trim, Tone.GlassDark, .08f, .9f, .47f, 1.4f, .02f);
                }
            }
            float back = -d / 2 + Shop / 2 + .5f;
            Wall[] shop = Shapes.Box(kit.Solid, lot, Tone.Cream, new Vector3(0, 0, back), new Vector3(w - 3, 3.4f, Shop), Tone.Slate);
            shop[0].Panel(kit.Trim, Tone.Glass, 1, .5f, w - 6, 2.5f);
            shop[0].Panel(kit.Trim, Tone.Door, w - 5.6f, 0, w - 4.2f, 2.3f);
            shop[0].Awning(kit.Trim, Tone.Teal, 0, w - 3, 2.8f, 1.3f, .25f);
            // The totem: prices on a navy slab by the street.
            Wall[] totem = Shapes.Box(kit.Solid, lot, Tone.Navy, new Vector3(-w / 2 + 1.2f, 0, d / 2 - 1), new Vector3(1.6f, 6.5f, .4f), Tone.Orange);
            for (int row = 0; row < 3; row++)
                totem[0].Panel(kit.Trim, row == 0 ? Tone.Orange : Tone.Yellow, .2f, 5.2f - row * 1.3f, 1.4f, 6.1f - row * 1.3f, .03f);
        }
    }
}
