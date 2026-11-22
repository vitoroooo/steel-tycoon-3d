using UnityEngine;
using Steel.Editor.Plan.Town;
using Steel.Presentation.World;

namespace Steel.Editor.Plan.Places.Builders
{
    /// <summary>Land kept for players: a fenced, numbered plot each. The city is shared, so nothing is built here ahead of them.</summary>
    internal static class Plots
    {
        private static Transform group;
        private static int lapak, factory;

        internal static void Reset()
        {
            group = new GameObject("Kavling pemain").transform;
            group.SetParent(CityGeometry.Root, false);
            lapak = factory = 0;
        }

        internal static void Lapak(TownKit kit, Lot lot) => Mark(kit, lot, PlayerPlot.Lapak, ++lapak, "KAVLING LAPAK ", Tone.Teal);

        internal static void Factory(TownKit kit, Lot lot) => Mark(kit, lot, PlayerPlot.Factory, ++factory, "KAVLING PABRIK ", Tone.Navy);

        private static void Mark(TownKit kit, Lot lot, string kind, int number, string title, Tone fence)
        {
            Yards.Fenced(kit, lot, Tone.Dirt, fence);
            Yards.Hut(kit, lot);
            bool mine = kind == PlayerPlot.Lapak && number == 1;   // the first yard lot is the local player's
            Props.Board(kit, lot.Part(-Yards.Gate / 2 - 4, lot.Depth / 2 + .6f, 6, 1), title + number + (mine ? "\nLAPAK ANDA" : "\nDISEWAKAN"));
            if (mine) Haul.YardWorksBuilder.Build(kit, lot);
            var plot = new GameObject(title + number).AddComponent<PlayerPlot>();
            plot.transform.SetParent(group, false);
            plot.transform.SetPositionAndRotation(lot.Point(0, 0, 0), Quaternion.Euler(0, lot.Yaw, 0));
            plot.Kind = kind; plot.Number = number; plot.Size = new Vector2(lot.Width, lot.Depth);
        }
    }
}
