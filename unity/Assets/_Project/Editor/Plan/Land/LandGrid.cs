using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Steel.Editor.Plan.Town;
using Steel.Services.Map;

namespace Steel.Editor.Plan.Land
{
    /// <summary>Who owns each square metre and what is already built on it. Nothing is placed without asking here first.</summary>
    internal sealed class LandGrid
    {
        private const float Sample = .7f, OldTownEdge = 2;
        internal readonly MapPlan Plan;
        internal readonly RoadField Roads;
        private readonly MapLand land;
        private readonly HashSet<int> taken = new HashSet<int>();
        private readonly List<Vector2>[] cells;

        internal LandGrid(MapPlan plan)
        {
            Plan = plan; land = new MapLand(plan); Roads = new RoadField(plan, land); cells = Survey();
        }

        /// <summary>True where this place may still build: its own land, outside the hand-built old town, not yet used.</summary>
        internal bool Free(int place, Vector2 spot) =>
            land.PlaceAt(spot.x, spot.y) == place && !Plan.core.Holds(spot.x, spot.y, OldTownEdge) && !taken.Contains(Key(spot));

        internal int Owner(Vector2 spot) => land.PlaceAt(spot.x, spot.y);

        internal bool Fits(int place, Lot lot, float gap)
        {
            foreach (Vector2 spot in Spots(lot, gap))
                if (!Free(place, spot)) return false;
            return true;
        }

        internal void Take(Lot lot, float gap)
        {
            foreach (Vector2 spot in Spots(lot, gap)) taken.Add(Key(spot));
        }

        /// <summary>The middle of every square metre of a place, the ones nearest a street first.</summary>
        internal IReadOnlyList<Vector2> Cells(int place) => cells[place];

        private List<Vector2>[] Survey()
        {
            var found = Plan.places.Select(_ => new List<Vector2>()).ToArray();
            for (float z = Plan.bounds.south + .5f; z < Plan.bounds.north; z++)
                for (float x = Plan.bounds.west + .5f; x < Plan.bounds.east; x++)
                {
                    int place = land.PlaceAt(x, z);
                    if (place != MapLand.NoPlace && !Plan.core.Holds(x, z, OldTownEdge)) found[place].Add(new Vector2(x, z));
                }
            return found.Select(list => list.OrderBy(cell => Mathf.Round(Roads.ToRoad(cell).magnitude))
                .ThenBy(cell => cell.x).ThenBy(cell => cell.y).ToList()).ToArray();
        }

        private static IEnumerable<Vector2> Spots(Lot lot, float gap)
        {
            float w = lot.Width / 2 + gap, d = lot.Depth / 2 + gap;
            for (float right = -w; right <= w + .01f; right += Mathf.Min(Sample, w * 2))
                for (float forward = -d; forward <= d + .01f; forward += Mathf.Min(Sample, d * 2))
                {
                    Vector3 point = lot.Point(right, 0, forward);
                    yield return new Vector2(point.x, point.z);
                }
        }

        private static int Key(Vector2 spot) => Mathf.FloorToInt(spot.y + 4096) * 8192 + Mathf.FloorToInt(spot.x + 4096);
    }
}
