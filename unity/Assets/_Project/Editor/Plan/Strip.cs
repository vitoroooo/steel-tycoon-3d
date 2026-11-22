using System;
using System.Collections.Generic;
using UnityEngine;

namespace Steel.Editor.Plan
{
    /// <summary>Collects bands laid along ways and builds them as one mesh per map sector, so far streets are never drawn.</summary>
    internal sealed class Strip
    {
        internal const float Sector = 64;
        /// <summary>Everything built from strips hangs here; the ground mask reads these meshes quad by quad.</summary>
        internal static Transform Parent;
        private readonly Dictionary<Vector2Int, StripBatch> sectors = new Dictionary<Vector2Int, StripBatch>();

        /// <summary>A band between two side offsets; pieces for which keep answers false are left out.</summary>
        internal void Lay(WayLine line, float fromOffset, float fromHeight, float toOffset, float toHeight,
            Func<int, bool> keep = null)
        {
            for (int piece = 0; piece < line.Pieces; piece++)
            {
                if (keep != null && !keep(piece)) continue;
                Quad(line.At(piece, fromOffset, fromHeight), line.At(piece + 1, fromOffset, fromHeight),
                    line.At(piece + 1, toOffset, toHeight), line.At(piece, toOffset, toHeight));
            }
        }

        /// <summary>Corners in clockwise order seen from the side the face should show; tint picks a swatch of a palette map.</summary>
        internal void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector2 tint = default)
        {
            var key = new Vector2Int(Mathf.FloorToInt(a.x / Sector), Mathf.FloorToInt(a.z / Sector));
            if (!sectors.TryGetValue(key, out StripBatch batch)) sectors[key] = batch = new StripBatch();
            batch.Add(a, b, c, d, tint);
        }

        internal void Build(string name, Material material, bool solid, bool shadows = false)
        {
            foreach (KeyValuePair<Vector2Int, StripBatch> sector in sectors)
            {
                string title = name + " " + sector.Key.x + "," + sector.Key.y;
                Mesh mesh = StripMesh.Create(title, sector.Value);
                var go = new GameObject(title, typeof(MeshFilter), typeof(MeshRenderer));
                go.transform.SetParent(Parent, false);
                go.GetComponent<MeshFilter>().sharedMesh = mesh;
                var renderer = go.GetComponent<MeshRenderer>();
                renderer.sharedMaterial = material;
                renderer.shadowCastingMode = shadows ? UnityEngine.Rendering.ShadowCastingMode.On
                    : UnityEngine.Rendering.ShadowCastingMode.Off;
                if (solid) go.AddComponent<MeshCollider>().sharedMesh = mesh;
                go.isStatic = true;
            }
        }
    }
}
