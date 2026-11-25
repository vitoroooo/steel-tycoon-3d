using System;
using UnityEngine;

namespace Steel.Services.Map
{
    /// <summary>A lot the plan fixes by hand because play depends on where it is, such as the truck dealer.</summary>
    [Serializable]
    public sealed class MapSite
    {
        public const string TruckDealer = "dealer-truk", PlayerLapak = "kavling-lapak", PlayerFactory = "kavling-pabrik",
            MillGate = "gerbang-pabrik";
        /// <summary>Where the loading or weighing post stands on a yard or gate lot, measured from its middle.</summary>
        public const float PostRight = -7, PostForward = 5;

        public string name, kind;
        /// <summary>The middle of the lot, and the compass bearing from it to its street.</summary>
        public float x, z, yaw;
        /// <summary>Frontage and depth in metres.</summary>
        public float width, depth;

        public Vector2 Centre => new Vector2(x, z);
        public Vector2 Front => new Vector2(Mathf.Sin(yaw * Mathf.Deg2Rad), Mathf.Cos(yaw * Mathf.Deg2Rad));

        /// <summary>A map point measured on the lot: right along the street, forward towards it.</summary>
        public Vector2 Point(float right, float forward) => Centre + new Vector2(Front.y, -Front.x) * right + Front * forward;

        public bool Holds(float px, float pz)
        {
            Vector2 offset = new Vector2(px, pz) - Centre;
            return Mathf.Abs(Vector2.Dot(offset, Front)) <= depth / 2 && Mathf.Abs(Vector2.Dot(offset, new Vector2(Front.y, -Front.x))) <= width / 2;
        }
    }
}
