using System.Linq;
using UnityEngine;
using Steel.Services.Map;

namespace Steel.Editor.Ground
{
    /// <summary>Reads the city plan for the ground: which district a spot is in, and how deep the river is dug there.</summary>
    internal sealed class GroundDistricts
    {
        private const float BankSlope = 1.2f;   // metres over which the bed rises to the channel wall
        private readonly MapPlan plan;
        private readonly MapLand land;
        private readonly MapWay river;
        private readonly float riverSouth, riverNorth;

        internal GroundDistricts()
        {
            plan = MapPlan.Load(); land = new MapLand(plan);
            river = plan.ways.First(way => way.kind == MapPlan.River);
            riverSouth = river.zs.Min() - river.width; riverNorth = river.zs.Max() + river.width;
        }

        /// <summary>The old town, whose ground is still described by hand in GroundZones.</summary>
        internal bool Core(float x, float z) => plan.core.Holds(x, z);

        /// <summary>The plan's district code here ("kampung", "industri", ...), or null on a way or verge.</summary>
        internal string Area(float x, float z)
        {
            int place = land.PlaceAt(x, z);
            return place == MapLand.NoPlace ? null : plan.places[place].area;
        }

        /// <summary>Metres to dig below city level: the full depth across the river, nothing outside its walls.</summary>
        internal float Dig(float x, float z, float depth)
        {
            if (z < riverSouth || z > riverNorth) return 0;
            float nearest = float.MaxValue;
            var point = new Vector2(x, z);
            for (int i = 0; i < river.xs.Length; i++)
                nearest = Mathf.Min(nearest, (river.Point(i) - point).sqrMagnitude);
            return depth * Mathf.InverseLerp(river.width / 2, river.width / 2 - BankSlope, Mathf.Sqrt(nearest));
        }
    }
}
