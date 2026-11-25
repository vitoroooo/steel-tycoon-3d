using UnityEngine;
using Steel.Editor.Plan.Land;
using Steel.Editor.Plan.Places.Builders;
using Steel.Editor.Plan.Town;
using Steel.Services.Map;

namespace Steel.Editor.Plan.Places
{
    /// <summary>Builds up every place on the plan until none of its land is left bare: a name board, lots, then small things.</summary>
    internal static class PlaceFiller
    {
        private static readonly Vector2[] BoardSize = { new Vector2(7, 1.4f) };

        internal static void Create(MapPlan plan)
        {
            var grid = new LandGrid(plan);
            var kit = new TownKit();
            int lots = Sites.Create(grid, kit), extras = 0;
            var doors = new Doors.DoorLedger();
            for (int place = 0; place < plan.places.Length; place++)
            {
                StyleSheet style = StyleBook.Of(plan.places[place]);
                Carpet.Lay(kit, grid, place);
                foreach (Lot board in LotPacker.Pack(grid, place, BoardSize, .2f, 1))
                    Props.Board(kit, board, StyleBook.Title(plan.places[place]));
                if (Doors.DoorBook.Of(plan.places[place]) is Steel.Core.Trade.Street.SellerKind kind)
                    foreach (Lot stall in LotPacker.Pack(grid, place, Doors.DoorStall.Size, .3f, 1))
                        Doors.DoorStall.Build(kit, stall, kind, place, doors);
                int built = 0;
                foreach (Lot lot in LotPacker.Pack(grid, place, style.Sizes, style.Gap))
                {
                    (built++ == 0 && style.Landmark != null ? style.Landmark : style.Build)(kit, lot); lots++;
                }
                extras += Scatter(grid, kit, place, style);
            }
            kit.Build();
            doors.Write();
            Debug.Log("PLACES_FILLED: " + plan.places.Length + " places, " + lots + " lots, " + extras + " small things, " + doors.Count + " doors");
        }

        private static int Scatter(LandGrid grid, TownKit kit, int place, StyleSheet style)
        {
            int count = 0;
            foreach (Vector2 cell in grid.Cells(place))
            {
                if (Mathf.FloorToInt(cell.x) % style.Spacing != 0 || Mathf.FloorToInt(cell.y) % style.Spacing != 0) continue;
                int seed = place * 31 + Mathf.RoundToInt(cell.x) * 977 + Mathf.RoundToInt(cell.y) * 359;
                Vector2 toRoad = grid.Roads.ToRoad(cell);
                var probe = new Lot(cell, toRoad.normalized, 1.4f, 1.4f, seed);
                var spot = new Lot(cell + new Vector2(probe.Roll(1) - .5f, probe.Roll(2) - .5f), probe.Front, 1.4f, 1.4f, seed);
                if (!grid.Fits(place, spot, .2f)) continue;
                grid.Take(spot, .2f); style.Scatter(kit, spot); count++;
            }
            return count;
        }
    }
}
