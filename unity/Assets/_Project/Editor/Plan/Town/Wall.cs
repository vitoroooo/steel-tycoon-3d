using UnityEngine;

namespace Steel.Editor.Plan.Town
{
    /// <summary>One upright face of a building, measured from its bottom corner: panels painted on it follow it anywhere.</summary>
    internal readonly struct Wall
    {
        internal readonly Vector3 Origin, Along, Out;
        internal readonly float Width, Height;

        internal Wall(Vector3 origin, Vector3 along, Vector3 outward, float width, float height)
        {
            Origin = origin; Along = along; Out = outward; Width = width; Height = height;
        }

        internal Vector3 At(float along, float up, float lift = 0) => Origin + Along * along + Vector3.up * up + Out * lift;

        /// <summary>A flat panel on the wall, lifted off it so it never flickers against the wall behind.</summary>
        internal void Panel(Strip strip, Tone tone, float left, float bottom, float right, float top, float lift = .05f) =>
            Shapes.Face(strip, tone, At(left, bottom, lift), At(left, top, lift), At(right, top, lift), At(right, bottom, lift), Out);

        /// <summary>Rows of evenly spread panels: windows on every floor.</summary>
        internal void Rows(Strip strip, Tone tone, int columns, int rows, float first, float pitch, float tall, float share = .6f)
        {
            if (columns < 1) return;
            float cell = Width / columns, inset = cell * (1 - share) / 2;
            for (int row = 0; row < rows; row++)
                for (int column = 0; column < columns; column++)
                    Panel(strip, tone, column * cell + inset, first + row * pitch, (column + 1) * cell - inset, first + row * pitch + tall);
        }

        /// <summary>A sloped sheet hanging off the wall: an awning or a canopy.</summary>
        internal void Awning(Strip strip, Tone tone, float left, float right, float height, float reach, float drop = .5f)
        {
            Vector3 a = At(left, height, .02f), b = At(right, height, .02f);
            Vector3 fall = Out * reach + Vector3.down * drop;
            Shapes.Face(strip, tone, a, b, b + fall, a + fall, Vector3.up);
            Shapes.Face(strip, tone, a, b, b + fall, a + fall, Vector3.down);
        }
    }
}
