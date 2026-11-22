using System;
using UnityEngine;
using Steel.Editor.Plan.Town;

namespace Steel.Editor.Plan.Places
{
    /// <summary>How one kind of place is built up: its lot sizes, the room between lots, and what fills the corners left over.</summary>
    internal sealed class StyleSheet
    {
        /// <summary>Lot sizes as (frontage, depth) in metres, largest first.</summary>
        internal readonly Vector2[] Sizes;
        /// <summary>Half the width of the gang left between neighbouring lots.</summary>
        internal readonly float Gap;
        /// <summary>Metres between the small things scattered over what the lots left free.</summary>
        internal readonly int Spacing;
        internal readonly Action<TownKit, Lot> Build, Scatter;

        internal StyleSheet(Action<TownKit, Lot> build, Action<TownKit, Lot> scatter, float gap, int spacing, params Vector2[] sizes)
        {
            Build = build; Scatter = scatter; Gap = gap; Spacing = spacing; Sizes = sizes;
        }
    }
}
