using UnityEngine;
using Steel.Editor.Plan.Water;

namespace Steel.Editor.Plan.Roads
{
    /// <summary>Where a way crosses the river: a parapet on both edges and a girder face down to the water.</summary>
    internal static class Bridge
    {
        private const float Parapet = .95f, Thickness = .35f, Approach = 2.5f, JoinRoom = 1;

        /// <param name="edge">Distance from the way's centre line to the outside of its deck.</param>
        /// <param name="deck">Height of the deck surface at that edge.</param>
        internal static void Create(Paving paving, WayLine line, float edge, float deck, RoadKit kit)
        {
            foreach (WayLine river in paving.Rivers)
                foreach (int side in new[] { -1, 1 })
                {
                    float inner = side * (edge - Thickness), outer = side * edge, top = deck + Parapet;
                    // No parapet across the mouth of a street that joins on the bridge.
                    bool Over(int piece) => Paving.Near(line.Middle(piece) + line.Sides[piece] * outer, river,
                        river.Way.width / 2 + Approach) && !paving.Crossed(line.Middle(piece) + line.Sides[piece] * outer, line, JoinRoom);
                    void Band(float from, float fromHeight, float to, float toHeight)
                    {
                        if (side > 0) kit.Concrete.Lay(line, from, fromHeight, to, toHeight, Over);
                        else kit.Concrete.Lay(line, to, toHeight, from, fromHeight, Over);
                    }
                    Band(inner, deck, inner, top);
                    Band(inner, top, outer, top);
                    Band(outer, top, outer, RiverBuilder.Surface - .2f);
                }
        }
    }
}
