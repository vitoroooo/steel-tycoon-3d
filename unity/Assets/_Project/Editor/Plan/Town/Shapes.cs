using UnityEngine;

namespace Steel.Editor.Plan.Town
{
    /// <summary>The few solids the town is made of. Every face is told which way to look, so winding is never a caller's worry.</summary>
    internal static class Shapes
    {
        internal static void Face(Strip strip, Tone tone, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 outward)
        {
            if ((c - d).sqrMagnitude < 1e-8f) Tri(strip, tone, a, b, c, outward);
            else if (Vector3.Dot(Vector3.Cross(c - a, d - a), outward) >= 0) strip.Quad(a, b, c, d, Tones.Uv(tone));
            else strip.Quad(a, d, c, b, Tones.Uv(tone));
        }

        /// <summary>A triangle, stored as a quad whose first edge has no length.</summary>
        internal static void Tri(Strip strip, Tone tone, Vector3 a, Vector3 b, Vector3 c, Vector3 outward)
        {
            if (Vector3.Dot(Vector3.Cross(b - a, c - a), outward) >= 0) strip.Quad(a, a, b, c, Tones.Uv(tone));
            else strip.Quad(a, a, c, b, Tones.Uv(tone));
        }

        /// <summary>Four walls on a lot (front, right, back, left) and, with a top tone, a lid. Centre.y is the foot.</summary>
        internal static Wall[] Box(Strip strip, Lot lot, Tone tone, Vector3 centre, Vector3 size, Tone? top = null)
        {
            float w = size.x / 2, d = size.z / 2, foot = centre.y, head = centre.y + size.y;
            Vector3 right = lot.Direction(1, 0), front = lot.Direction(0, 1);
            var walls = new[]
            {
                new Wall(lot.Point(centre.x - w, foot, centre.z + d), right, front, size.x, size.y),
                new Wall(lot.Point(centre.x + w, foot, centre.z + d), -front, right, size.z, size.y),
                new Wall(lot.Point(centre.x + w, foot, centre.z - d), -right, -front, size.x, size.y),
                new Wall(lot.Point(centre.x - w, foot, centre.z - d), front, -right, size.z, size.y)
            };
            // Whatever stands on the ground runs a little below it, so no gap shows on uneven terrain.
            float skirt = foot < .01f ? Lot.Foot - foot : 0;
            foreach (Wall wall in walls) wall.Panel(strip, tone, 0, skirt, wall.Width, wall.Height, 0);
            if (top.HasValue)
                Flat(strip, lot, top.Value, centre.x, centre.z, size.x, size.z, head);
            return walls;
        }

        /// <summary>A level rectangle: a lid, a floor, a pond, a pitch.</summary>
        internal static void Flat(Strip strip, Lot lot, Tone tone, float x, float z, float width, float depth, float height)
        {
            float w = width / 2, d = depth / 2;
            Face(strip, tone, lot.Point(x - w, height, z - d), lot.Point(x - w, height, z + d),
                lot.Point(x + w, height, z + d), lot.Point(x + w, height, z - d), Vector3.up);
        }

        /// <summary>A pitched roof with its ridge along the street. Hip pulls the ridge ends in; at zero the ends are gables.</summary>
        internal static void Roof(Strip strip, Lot lot, Tone tone, Tone end, Vector3 centre, float width, float depth, float rise, float hip)
        {
            float w = width / 2, d = depth / 2, eave = centre.y, ridge = centre.y + rise;
            hip = Mathf.Min(hip, w);   // a roof deeper than it is wide comes to a point instead of crossing over
            Vector3 left = lot.Point(centre.x - w + hip, ridge, centre.z), right = lot.Point(centre.x + w - hip, ridge, centre.z);
            foreach (int side in new[] { -1, 1 })
            {
                Vector3 a = lot.Point(centre.x - w, eave, centre.z + side * d), b = lot.Point(centre.x + w, eave, centre.z + side * d);
                Face(strip, tone, a, b, right, left, Vector3.up);
                Vector3 tip = side < 0 ? left : right;
                Vector3 near = lot.Point(centre.x + side * w, eave, centre.z - d), far = lot.Point(centre.x + side * w, eave, centre.z + d);
                Tri(strip, hip > 0 ? tone : end, near, far, tip, lot.Direction(side, 0) + Vector3.up * .01f);
            }
        }

        /// <summary>An upright round shape: a trunk, a tank, a chimney, a tree crown, a heap. Zero top radius makes a cone.</summary>
        internal static void Drum(Strip strip, Tone tone, Vector3 foot, float lower, float upper, float height, int sides = 8)
        {
            Vector3 head = foot + Vector3.up * height;
            for (int i = 0; i < sides; i++)
            {
                Vector3 from = Spoke(i, sides), to = Spoke(i + 1, sides);
                Face(strip, tone, foot + from * lower, foot + to * lower, head + to * upper, head + from * upper, from + to + Vector3.up * .01f);
                if (upper > 0) Tri(strip, tone, head, head + from * upper, head + to * upper, Vector3.up);
            }
        }

        private static Vector3 Spoke(int index, int sides)
        {
            float angle = index * Mathf.PI * 2 / sides;
            return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        }
    }
}
