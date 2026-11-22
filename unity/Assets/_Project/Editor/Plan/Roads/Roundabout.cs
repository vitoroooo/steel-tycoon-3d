using UnityEngine;
using Steel.Services.Map;

namespace Steel.Editor.Plan.Roads
{
    /// <summary>A roundabout: a disc of asphalt above the streets that meet there, with a kerbed green island in the middle.</summary>
    internal static class Roundabout
    {
        internal const float IslandShare = .45f;
        private const float Top = .07f, IslandHeight = .3f;
        private const int Slices = 48;

        internal static void Create(MapRoundabout ring, RoadKit kit)
        {
            var centre = new Vector3(ring.x, 0, ring.z);
            float island = ring.radius * IslandShare, lip = island - RoadBuilder.KerbWidth * 2;
            Band(kit.Asphalt, centre, island - .1f, Top, ring.radius, Top);
            Band(kit.Concrete, centre, island, Top + IslandHeight, island, Top);
            Band(kit.Concrete, centre, lip, Top + IslandHeight, island, Top + IslandHeight);
            Band(kit.Grass, centre, 0, Top + IslandHeight, lip, Top + IslandHeight);
            Band(kit.White, centre, island + .5f, Top + .012f, island + .7f, Top + .012f);
            Band(kit.White, centre, ring.radius - .65f, Top + .012f, ring.radius - .45f, Top + .012f, 2);
        }

        /// <summary>A ring between two radii; with dash above one, only every dash-th slice is laid.</summary>
        private static void Band(Strip strip, Vector3 centre, float inner, float innerHeight, float outer, float outerHeight,
            int dash = 1)
        {
            for (int slice = 0; slice < Slices; slice += dash)
            {
                Vector3 from = Spoke(slice), to = Spoke(slice + 1);
                strip.Quad(centre + from * inner + Vector3.up * innerHeight, centre + to * inner + Vector3.up * innerHeight,
                    centre + to * outer + Vector3.up * outerHeight, centre + from * outer + Vector3.up * outerHeight);
            }
        }

        // Counter-clockwise seen from above, so bands laid from the inside out face up.
        private static Vector3 Spoke(int slice)
        {
            float angle = slice * Mathf.PI * 2 / Slices;
            return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        }
    }
}
