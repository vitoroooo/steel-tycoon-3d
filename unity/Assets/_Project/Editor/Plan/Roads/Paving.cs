using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Steel.Services.Map;

namespace Steel.Editor.Plan.Roads
{
    /// <summary>Knows where asphalt lies, so kerbs, pavements and markings stop wherever another street crosses.</summary>
    internal sealed class Paving
    {
        internal readonly MapPlan Plan;
        internal readonly List<WayLine> Roads, Rails, Rivers;

        internal Paving(MapPlan plan)
        {
            Plan = plan;
            Roads = Lines(way => way.IsRoad); Rails = Lines(way => way.kind == MapPlan.Rail);
            Rivers = Lines(way => way.kind == MapPlan.River);
        }

        private List<WayLine> Lines(System.Func<MapWay, bool> wanted) =>
            Plan.ways.Where(wanted).Select(way => new WayLine(way)).ToList();

        /// <summary>True within reach of a way's centre line.</summary>
        internal static bool Near(Vector2 point, WayLine line, float reach) =>
            line.Points.Any(other => (other - point).sqrMagnitude < reach * reach);

        /// <summary>True inside a street of the old town, which is already built and must stay as it is.</summary>
        internal bool Built(Vector2 point, float margin = 0) =>
            Plan.built.Any(rect => rect.Holds(point.x, point.y, margin));

        /// <summary>True when the point lies on (or within margin of) the asphalt of any street other than its own.</summary>
        internal bool Crossed(Vector2 point, WayLine own, float margin)
        {
            if (Built(point, margin)) return true;
            foreach (MapRoundabout ring in Plan.roundabouts)
                if (Vector2.Distance(point, new Vector2(ring.x, ring.z)) < ring.radius + margin) return true;
            foreach (WayLine road in Roads)
            {
                if (road == own) continue;
                float reach = road.Way.width / 2 + margin; reach *= reach;
                foreach (Vector2 other in road.Points)
                    if ((other - point).sqrMagnitude < reach) return true;
            }
            return false;
        }
    }
}
