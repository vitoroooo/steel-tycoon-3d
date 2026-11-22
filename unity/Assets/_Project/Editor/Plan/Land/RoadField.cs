using System.Collections.Generic;
using UnityEngine;
using Steel.Services.Map;

namespace Steel.Editor.Plan.Land
{
    /// <summary>For every square metre of the map: which bit of street is nearest. Buildings use it to face their street.</summary>
    internal sealed class RoadField
    {
        private readonly int[] nearest;
        private readonly int width, height;
        private readonly MapRect bounds;

        internal RoadField(MapPlan plan, MapLand land)
        {
            bounds = plan.bounds;
            width = Mathf.RoundToInt(bounds.east - bounds.west); height = Mathf.RoundToInt(bounds.north - bounds.south);
            nearest = new int[width * height];
            var frontier = new Queue<int>();
            for (int cell = 0; cell < nearest.Length; cell++)
            {
                bool road = land.IsRoad(bounds.west + cell % width + .5f, bounds.south + cell / width + .5f);
                nearest[cell] = road ? cell : -1;
                if (road) frontier.Enqueue(cell);
            }
            // Spreading outwards from every street at once reaches each cell first from the street closest to it.
            while (frontier.Count > 0)
            {
                int cell = frontier.Dequeue(), column = cell % width, row = cell / width;
                for (int dx = -1; dx <= 1; dx++)
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        int x = column + dx, z = row + dz, next = z * width + x;
                        if (x < 0 || z < 0 || x >= width || z >= height || nearest[next] >= 0) continue;
                        nearest[next] = nearest[cell]; frontier.Enqueue(next);
                    }
            }
        }

        /// <summary>From a spot to the nearest street: the vector's length is the distance.</summary>
        internal Vector2 ToRoad(Vector2 spot)
        {
            int column = Mathf.Clamp(Mathf.FloorToInt(spot.x - bounds.west), 0, width - 1);
            int row = Mathf.Clamp(Mathf.FloorToInt(spot.y - bounds.south), 0, height - 1);
            int road = nearest[row * width + column];
            if (road < 0) return Vector2.up * 1000;
            return new Vector2(bounds.west + road % width + .5f, bounds.south + road / width + .5f) - spot;
        }
    }
}
