using System.Linq;
using UnityEngine;
using Steel.Editor.Plan.Land;
using Steel.Editor.Plan.Town;
using Steel.Services.Map;

namespace Steel.Editor.Plan.Places
{
    /// <summary>The ground of a place in its district's colour, so the land between buildings reads as plaza, yard or lawn.</summary>
    internal static class Carpet
    {
        private const float Tuck = .6f, Level = -.085f;   // tucked under the pavement around it, just over the terrain

        internal static Tone Of(MapPlace place)
        {
            switch (place.area)
            {
                case "pusat": case "pasar": return Tone.Paving;
                case "industri": return Tone.Concrete;
                case "pinggir": return Tone.Grass;
                default: return Tone.Dirt;
            }
        }

        /// <summary>One quad per unbroken run of the place's land in each row of the map.</summary>
        internal static void Lay(TownKit kit, LandGrid grid, int place)
        {
            Tone tone = Of(grid.Plan.places[place]);
            float height = Level - place % 4 * .004f;   // neighbours overlap where they meet; a step apart, they never flicker
            foreach (IGrouping<float, Vector2> row in grid.Cells(place).GroupBy(cell => cell.y))
            {
                float[] xs = row.Select(cell => cell.x).OrderBy(x => x).ToArray();
                for (int from = 0, i = 1; i <= xs.Length; i++)
                {
                    if (i < xs.Length && xs[i] - xs[i - 1] < 1.5f) continue;
                    float west = xs[from] - .5f - Tuck, east = xs[i - 1] + .5f + Tuck, south = row.Key - .5f - Tuck, north = row.Key + .5f + Tuck;
                    Shapes.Face(kit.Trim, tone, new Vector3(west, height, south), new Vector3(west, height, north),
                        new Vector3(east, height, north), new Vector3(east, height, south), Vector3.up);
                    from = i;
                }
            }
        }
    }
}
