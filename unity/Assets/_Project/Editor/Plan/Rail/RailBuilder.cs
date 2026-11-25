using UnityEngine;
using Steel.Editor.Plan.Roads;
using Steel.Editor.Toon;

namespace Steel.Editor.Plan.Rail
{
    /// <summary>The railway: a ballast bed, sleepers and two rails. Streets cross it level, with the rails showing in the asphalt.</summary>
    internal static class RailBuilder
    {
        private const float Bed = .03f, SleeperTop = .05f, RailTop = .13f;
        private const float Gauge = 1.07f, RailWidth = .1f, SleeperHalf = 1.1f, BedHalf = 2.2f;

        internal static void Create(WayLine rail, Paving paving, RoadKit kit)
        {
            Strip ballast = new Strip(), sleepers = new Strip(), steel = new Strip();
            ballast.Lay(rail, -BedHalf, Bed, BedHalf, Bed);
            ballast.Lay(rail, -BedHalf - .6f, RoadBuilder.GroundLevel, -BedHalf, Bed);
            ballast.Lay(rail, BedHalf, Bed, BedHalf + .6f, RoadBuilder.GroundLevel);
            for (int piece = 0; piece < rail.Pieces; piece++)
            {
                Vector2 middle = rail.Middle(piece), side = rail.Sides[piece], along = rail.Heading(piece) * .14f;
                sleepers.Quad(Flat(middle - side * SleeperHalf - along), Flat(middle - side * SleeperHalf + along),
                    Flat(middle + side * SleeperHalf + along), Flat(middle + side * SleeperHalf - along));
            }
            foreach (int side in new[] { -1, 1 })
            {
                float centre = side * Gauge / 2;
                steel.Lay(rail, centre - RailWidth / 2, RailTop, centre + RailWidth / 2, RailTop);
                steel.Lay(rail, centre - RailWidth / 2, Bed, centre - RailWidth / 2, RailTop);
                steel.Lay(rail, centre + RailWidth / 2, RailTop, centre + RailWidth / 2, Bed);
            }
            Bridge.Create(paving, rail, BedHalf + .6f, Bed, kit);
            ballast.Build("Balas rel", CityMaterials.Surface("gravel_floor_02", Vector2.one, "Balas rel", ToonPalette.Hex("9C9484")), true);
            sleepers.Build("Bantalan rel", CityMaterials.Plain("Bantalan rel", ToonPalette.Hex("6B4A34")), false);
            steel.Build("Rel", CityMaterials.Plain("Baja rel", ToonPalette.Hex("3A4150")), false);
        }

        private static Vector3 Flat(Vector2 point) => new Vector3(point.x, SleeperTop, point.y);
    }
}
