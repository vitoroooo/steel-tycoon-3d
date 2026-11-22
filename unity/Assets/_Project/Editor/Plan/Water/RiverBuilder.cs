using UnityEngine;
using Steel.Editor.Plan.Roads;
using Steel.Editor.Toon;

namespace Steel.Editor.Plan.Water
{
    /// <summary>Kali Sukabaja: water in a walled channel, with a parapet wherever a street or the railway crosses.</summary>
    internal static class RiverBuilder
    {
        /// <summary>Water level and river bed; the terrain is dug to the bed along the same line.</summary>
        internal const float Surface = -1.1f, Bed = -2.4f;
        private const float WallTop = .75f, WallWidth = .5f;

        internal static void Create(WayLine river, Paving paving, RoadKit kit)
        {
            float half = river.Way.width / 2;
            var water = new Strip();
            water.Lay(river, -half, Surface, half, Surface);
            water.Build("Air kali", CityMaterials.Plain("Air kali", ToonPalette.Hex("5FB8D6")), false);
            foreach (int side in new[] { -1, 1 })
            {
                float inner = side * half, outer = side * (half + WallWidth);
                // Under a bridge the wall stops at deck level, so the street runs over it.
                bool Open(int piece)
                {
                    Vector2 point = river.Middle(piece) + river.Sides[piece] * inner;
                    return !paving.Crossed(point, null, 4.5f) && !paving.Rails.Exists(rail => Paving.Near(point, rail, 4));
                }
                void Band(float from, float fromHeight, float to, float toHeight, System.Func<int, bool> keep)
                {
                    if (side > 0) kit.Concrete.Lay(river, from, fromHeight, to, toHeight, keep);
                    else kit.Concrete.Lay(river, to, toHeight, from, fromHeight, keep);
                }
                Band(inner, Bed, inner, 0, null);
                Band(inner, 0, inner, WallTop, Open);
                Band(inner, WallTop, outer, WallTop, Open);
                Band(outer, WallTop, outer, RoadBuilder.GroundLevel, Open);
            }
        }
    }
}
