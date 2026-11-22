using System.Collections.Generic;
using UnityEngine;
using Steel.Services.Map;

namespace Steel.Editor.Plan
{
    /// <summary>A way cut into even steps, so strips laid along it bend smoothly and can be skipped piece by piece.</summary>
    internal sealed class WayLine
    {
        internal const float Step = 2;
        internal readonly MapWay Way;
        internal readonly Vector2[] Points, Sides;   // Sides point to the right of travel
        internal int Pieces => Points.Length - 1;

        internal WayLine(MapWay way)
        {
            Way = way;
            var points = new List<Vector2> { way.Point(0) };
            float owed = Step;
            for (int i = 1; i < way.xs.Length; i++)
            {
                Vector2 from = way.Point(i - 1), to = way.Point(i);
                float length = Vector2.Distance(from, to), walked = 0;
                while (length - walked >= owed)
                {
                    walked += owed; owed = Step;
                    points.Add(Vector2.Lerp(from, to, walked / length));
                }
                owed -= length - walked;
            }
            Vector2 last = way.Point(way.xs.Length - 1);
            if (Vector2.Distance(points[points.Count - 1], last) < Step / 2) points[points.Count - 1] = last;
            else points.Add(last);
            Points = points.ToArray();
            Sides = new Vector2[Points.Length];
            for (int i = 0; i < Points.Length; i++) Sides[i] = Side(i);
        }

        internal Vector2 Middle(int piece) => (Points[piece] + Points[piece + 1]) / 2;
        internal Vector2 Heading(int piece) => (Points[piece + 1] - Points[piece]).normalized;
        internal Vector3 At(int index, float offset, float height)
        {
            Vector2 point = Points[index] + Sides[index] * offset;
            return new Vector3(point.x, height, point.y);
        }

        private Vector2 Side(int index)
        {
            int count = Points.Length;
            Vector2 before = Points[Way.closed ? (index - 1 + count - 1) % (count - 1) : Mathf.Max(index - 1, 0)];
            Vector2 after = Points[Way.closed ? (index + 1) % (count - 1) : Mathf.Min(index + 1, count - 1)];
            Vector2 along = (after - before).normalized;
            return new Vector2(along.y, -along.x);
        }
    }
}
