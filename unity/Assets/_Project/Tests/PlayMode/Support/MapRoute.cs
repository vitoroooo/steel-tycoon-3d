using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Steel.Services.Map;

namespace Steel.PlayTests.Support
{
    /// <summary>Turns streets of the plan into waypoints, keeping to the left lane as traffic here does.</summary>
    internal static class MapRoute
    {
        internal static MapWay Way(MapPlan plan, string kind) => plan.ways.First(way => way.kind == kind);

        /// <summary>A point of the way, moved left of its direction of travel by lane metres.</summary>
        internal static Vector3 Lane(MapWay way, int index, float lane)
        {
            int count = way.xs.Length;
            int before = way.closed ? (index + count - 1) % count : Mathf.Max(0, index - 1);
            int after = way.closed ? (index + 1) % count : Mathf.Min(count - 1, index + 1);
            Vector2 heading = (way.Point(after) - way.Point(before)).normalized;
            Vector2 spot = way.Point(index % count) + new Vector2(-heading.y, heading.x) * lane;
            return new Vector3(spot.x, 0, spot.y);
        }

        /// <summary>Waypoints every few pieces from one index to another, travelling up the way.</summary>
        internal static Vector3[] Along(MapWay way, int from, int to, int step, float lane)
        {
            var route = new List<Vector3>();
            for (int index = from; index <= to; index += step) route.Add(Lane(way, index, lane));
            return route.ToArray();
        }

        /// <summary>The street and the index of its point nearest to a spot on the map.</summary>
        internal static (MapWay Way, int Index) Nearest(MapPlan plan, Vector2 spot)
        {
            MapWay best = null; int at = 0; float nearest = float.MaxValue;
            foreach (MapWay way in plan.ways.Where(way => way.IsRoad))
                for (int index = 0; index < way.xs.Length; index++)
                {
                    float gap = (way.Point(index) - spot).sqrMagnitude;
                    if (gap < nearest) { nearest = gap; best = way; at = index; }
                }
            return (best, at);
        }
    }
}
