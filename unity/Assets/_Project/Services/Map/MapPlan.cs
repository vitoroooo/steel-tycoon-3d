using System;
using UnityEngine;

namespace Steel.Services.Map
{
    /// <summary>The city plan the game is built from, baked by tools/denah/export.py. Metres: x east, z north.</summary>
    [Serializable]
    public sealed class MapPlan
    {
        public const string River = "kali", Rail = "rel", Collector = "kolektor", Artery = "arteri", Ring = "lingkar";

        public MapRect bounds, core;
        /// <summary>Streets of the old town that already stand; nothing new is laid inside them.</summary>
        public MapRect[] built;
        public MapWay[] ways;
        public MapPlace[] places;
        public MapRoundabout[] roundabouts;

        public static MapPlan Load()
        {
            TextAsset asset = Resources.Load<TextAsset>("Map/Sukabaja");
            if (asset == null) throw new InvalidOperationException("Missing map plan: run tools/denah/export.py");
            return JsonUtility.FromJson<MapPlan>(asset.text);
        }
    }

    [Serializable]
    public sealed class MapWay
    {
        public string name, kind;
        public float width;
        public bool closed;
        public float[] xs, zs;

        public bool IsRoad => kind != MapPlan.River && kind != MapPlan.Rail;
        public Vector2 Point(int index) => new Vector2(xs[index], zs[index]);
    }

    [Serializable]
    public sealed class MapPlace
    {
        public string name, detail, area;
        public float x, z;
        public int sqm;
    }

    [Serializable]
    public sealed class MapRoundabout
    {
        public float x, z, radius;
    }

    [Serializable]
    public struct MapRect
    {
        public float west, south, east, north;

        public bool Holds(float x, float z, float margin = 0) =>
            x > west - margin && x < east + margin && z > south - margin && z < north + margin;
    }
}
