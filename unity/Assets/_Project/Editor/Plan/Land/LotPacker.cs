using System.Collections.Generic;
using UnityEngine;
using Steel.Editor.Plan.Town;

namespace Steel.Editor.Plan.Land
{
    /// <summary>Lines a place's land with lots: along its streets first, then row behind row, the largest size first.</summary>
    internal static class LotPacker
    {
        private const float Turn = 3, Reach = 3;   // lots turn in steps of 3 degrees, so neighbours line up

        internal static List<Lot> Pack(LandGrid grid, int place, Vector2[] sizes, float gap, int limit = int.MaxValue)
        {
            var lots = new List<Lot>();
            foreach (Vector2 size in sizes)
                foreach (Vector2 cell in grid.Cells(place))
                {
                    if (lots.Count >= limit) return lots;
                    if (!grid.Free(place, cell)) continue;
                    int seed = place * 7919 + Mathf.RoundToInt(cell.x) * 613 + Mathf.RoundToInt(cell.y) * 127;
                    var probe = new Lot(cell, Facing(grid, cell), size.x, size.y, seed);
                    var lot = new Lot(cell, probe.Front, size.x * (.9f + probe.Roll(90) * .2f), size.y, seed);
                    if (!grid.Fits(place, lot, gap)) continue;
                    grid.Take(lot, gap); lots.Add(lot);
                }
            return lots;
        }

        /// <summary>The way to the street, averaged over a few metres of frontage and rounded to a step.</summary>
        private static Vector2 Facing(LandGrid grid, Vector2 cell)
        {
            Vector2 toRoad = grid.Roads.ToRoad(cell).normalized, along = new Vector2(toRoad.y, -toRoad.x) * Reach;
            Vector2 mean = toRoad + grid.Roads.ToRoad(cell + along).normalized + grid.Roads.ToRoad(cell - along).normalized;
            if (mean.sqrMagnitude < .01f) mean = Vector2.up;
            float angle = Mathf.Round(Mathf.Atan2(mean.y, mean.x) * Mathf.Rad2Deg / Turn) * Turn * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }
}
