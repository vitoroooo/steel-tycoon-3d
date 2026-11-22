using System;

namespace UnityEngine
{
    /// <summary>The little vector arithmetic the map plan needs.</summary>
    public struct Vector2
    {
        public float x, y;
        public Vector2(float x, float y) { this.x = x; this.y = y; }
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
        public static Vector2 operator *(Vector2 a, float k) => new Vector2(a.x * k, a.y * k);
        public static float Dot(Vector2 a, Vector2 b) => a.x * b.x + a.y * b.y;
    }

    public static class Mathf
    {
        public const float Deg2Rad = (float)(Math.PI / 180);
        public static float Sin(float value) => (float)Math.Sin(value);
        public static float Cos(float value) => (float)Math.Cos(value);
        public static float Abs(float value) => Math.Abs(value);
        public static int FloorToInt(float value) => (int)Math.Floor(value);
    }
}
