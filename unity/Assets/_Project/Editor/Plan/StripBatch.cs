using System.Collections.Generic;
using UnityEngine;

namespace Steel.Editor.Plan
{
    /// <summary>The quads of one strip that fall in one map sector: four corners and four palette coordinates each.</summary>
    internal sealed class StripBatch
    {
        internal readonly List<Vector3> Corners = new List<Vector3>();
        internal readonly List<Vector2> Tints = new List<Vector2>();

        internal void Add(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector2 tint)
        {
            Corners.Add(a); Corners.Add(b); Corners.Add(c); Corners.Add(d);
            for (int i = 0; i < 4; i++) Tints.Add(tint);
        }
    }
}
