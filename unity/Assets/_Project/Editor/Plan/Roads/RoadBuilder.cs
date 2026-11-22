using UnityEngine;

namespace Steel.Editor.Plan.Roads
{
    /// <summary>Lays every planned street: asphalt, raised pavements with painted kerbstones, and lane markings.</summary>
    internal static class RoadBuilder
    {
        internal const float KerbWidth = .22f;
        /// <summary>Just under the terrain: skirts run down to here so nothing shows a gap.</summary>
        internal static readonly float GroundLevel = Ground.GroundZones.CityLevel - .05f;

        internal static void Create(Paving paving, RoadKit kit)
        {
            foreach (WayLine line in paving.Roads)
            {
                RoadProfile profile = RoadProfile.Of(line.Way);
                float half = line.Way.width / 2;
                kit.Asphalt.Lay(line, -half, profile.Top, half, profile.Top, piece => !paving.Built(line.Middle(piece)));
                foreach (int side in new[] { -1, 1 })
                    Pavement(paving, line, profile, side, kit.Walk, kit.PaleKerb, kit.DarkKerb);
                Bridge.Create(paving, line, half + profile.Pavement, StreetBuilder.PavementTop, kit);
                Markings.Create(paving, line, profile, kit);
                if (profile.Median > 0) Median.Create(paving, line, profile, kit);
            }
        }

        private static void Pavement(Paving paving, WayLine line, RoadProfile profile, int side,
            Strip walk, Strip pale, Strip dark)
        {
            float inner = side * line.Way.width / 2, outer = inner + side * profile.Pavement;
            float kerb = inner + side * KerbWidth, top = StreetBuilder.PavementTop;
            bool Stands(int piece) => !paving.Crossed(line.Middle(piece) + line.Sides[piece] * (inner + outer) / 2,
                line, profile.Pavement / 2);
            // Faces must show upward and outward, so the left side is laid outside in.
            void Band(Strip strip, float from, float fromHeight, float to, float toHeight, System.Func<int, bool> keep)
            {
                if (side > 0) strip.Lay(line, from, fromHeight, to, toHeight, keep);
                else strip.Lay(line, to, toHeight, from, fromHeight, keep);
            }
            Band(walk, kerb, top, outer, top, Stands);
            Band(walk, outer, top, outer, GroundLevel, Stands);
            foreach (Strip stones in new[] { pale, dark })
            {
                bool Mine(int piece) => Stands(piece) && (piece % 2 == 0) == (stones == pale);
                Band(stones, inner, profile.Top, inner, top, Mine);
                Band(stones, inner, top, kerb, top, Mine);
            }
        }
    }
}
