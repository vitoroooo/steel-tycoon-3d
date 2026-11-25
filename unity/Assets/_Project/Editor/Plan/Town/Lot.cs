using UnityEngine;

namespace Steel.Editor.Plan.Town
{
    /// <summary>A rectangle of land facing a street. Builders work in its own frame: right along the street, forward towards it.</summary>
    internal readonly struct Lot
    {
        /// <summary>Just under the terrain, so nothing built on a lot shows a gap at its foot.</summary>
        internal const float Foot = -.2f, Floor = -.05f;
        internal readonly Vector2 Centre, Front;
        internal readonly float Width, Depth;
        internal readonly int Seed;

        internal Lot(Vector2 centre, Vector2 front, float width, float depth, int seed)
        {
            Centre = centre; Front = front; Width = width; Depth = depth; Seed = seed;
        }

        private Vector2 Right => new Vector2(Front.y, -Front.x);

        internal Vector3 Point(float right, float up, float forward)
        {
            Vector2 flat = Centre + Right * right + Front * forward;
            return new Vector3(flat.x, up, flat.y);
        }

        internal Vector3 Direction(float right, float forward)
        {
            Vector2 flat = Right * right + Front * forward;
            return new Vector3(flat.x, 0, flat.y);
        }

        /// <summary>Compass yaw of something on the lot that looks at the street.</summary>
        internal float Yaw => Mathf.Atan2(Front.x, Front.y) * Mathf.Rad2Deg;

        /// <summary>A part of this lot, facing the same way.</summary>
        internal Lot Part(float right, float forward, float width, float depth, int salt = 1) =>
            new Lot(Centre + Right * right + Front * forward, Front, width, depth, Seed * 31 + salt);

        /// <summary>A repeatable number from 0 to 1; each salt gives a different one for the same lot.</summary>
        internal float Roll(int salt)
        {
            uint hash = (uint)(Seed * 73856093) ^ (uint)(salt * 19349663);
            hash ^= hash >> 13; hash *= 0x5bd1e995; hash ^= hash >> 15;
            return (hash & 0xFFFF) / 65536f;
        }

        internal int Pick(int salt, int count) => Mathf.Min(count - 1, (int)(Roll(salt) * count));
        internal Tone Pick(int salt, Tone[] tones) => tones[Pick(salt, tones.Length)];
    }
}
