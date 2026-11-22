using UnityEngine;
using Steel.Editor.Toon;

namespace Steel.Editor.Vehicles
{
    /// <summary>The way out of the dealer: a yellow apron across the pavement and a ramp down to the main road.</summary>
    internal static class DealerDriveway
    {
        private const float KerbX = 7.05f, RoadTop = .06f, RampRun = 1.1f, LotEdge = 11;

        /// <param name="z">Centre of the driveway along the main road.</param>
        internal static void Create(float z, float width)
        {
            float top = StreetBuilder.PavementTop, rise = top - RoadTop;
            Material apron = CityMaterials.Plain("Dealer jalan keluar", ToonPalette.Yellow);
            // A thin slab on the pavement marks where vehicles cross; it sits 1 cm proud so it never flickers.
            CityGeometry.Box("Dealer / apron", new Vector3((KerbX + LotEdge) / 2, top, z),
                new Vector3(LotEdge - KerbX + .2f, .02f, width), apron, false);
            // The ramp starts on the asphalt and meets the pavement at the kerb line.
            GameObject ramp = CityGeometry.Box("Dealer / ramp", new Vector3(KerbX - RampRun / 2, (top + RoadTop) / 2 - .02f, z),
                new Vector3(Mathf.Sqrt(RampRun * RampRun + rise * rise), .06f, width), apron);
            ramp.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(rise, RampRun) * Mathf.Rad2Deg);
        }
    }
}
