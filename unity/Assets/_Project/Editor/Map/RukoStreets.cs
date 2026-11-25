using UnityEngine;

namespace Steel.Editor.Map
{
    /// <summary>Where the ruko terraces stand. Rows A and B are back to back and share the alley between them.</summary>
    internal static class RukoStreets
    {
        /// <summary>Centre line of the back alley between the two Pasar Lama rows, and the gap between their walls.</summary>
        internal const float AlleyX = -23.3f, AlleyWidth = 5;
        internal const float AlleySouth = 22, AlleyNorth = 57;
        private const float PasarEastX = AlleyX + (AlleyWidth + RukoRow.Depth) / 2;
        private const float PasarWestX = AlleyX - (AlleyWidth + RukoRow.Depth) / 2;
        /// <summary>The market lane: its western kerb, where the kampung begins, and its length.</summary>
        internal const float LaneWidth = 6, LaneSouth = -7, LaneNorth = 60;
        internal const float LaneWest = PasarWestX - RukoRow.FrontReach - LaneWidth;

        internal static void Create()
        {
            // A: faces the main road. B: faces the market lane. Seven units each, z 22 to 57.
            RukoRow.Create(new Vector3(PasarEastX, 0, AlleyNorth - 2.5f), 90, ShopNames.PasarTimur);
            RukoRow.Create(new Vector3(PasarWestX, 0, AlleySouth + 2.5f), 270, ShopNames.PasarBarat, 3);
            // C: workshops facing the cross road, backing onto the factory forecourt.
            RukoRow.Create(new Vector3(47, 0, -1.7f), 180, ShopNames.Bengkel, 1);
            // D: north of the factory, facing the main road.
            RukoRow.Create(new Vector3(16.3f, 0, 40.5f), 270, ShopNames.Industri, 4);
            MarketLane();
        }

        /// <summary>The concrete lane the western row opens onto; it meets the cross road at its south end.</summary>
        private static void MarketLane()
        {
            float width = LaneWidth, east = LaneWest + width, south = LaneSouth, north = LaneNorth;
            var concrete = CityMaterials.Surface("concrete_floor_02", new Vector2(2, (north - south) / 3),
                "Gang Pasar beton", new Color(.7f, .7f, .68f));
            CityGeometry.Box("Gang Pasar", new Vector3(east - width / 2, -.1f, (south + north) / 2),
                new Vector3(width, .2f, north - south), concrete);
        }
    }
}
