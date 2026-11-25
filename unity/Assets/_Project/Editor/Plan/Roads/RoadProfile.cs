using Steel.Services.Map;

namespace Steel.Editor.Plan.Roads
{
    /// <summary>The cross-section of each street class: how wide its pavements are and what is painted on it.</summary>
    internal readonly struct RoadProfile
    {
        /// <summary>Asphalt surface height. Classes differ by millimetres so crossing streets never flicker.</summary>
        internal readonly float Top;
        internal readonly float Pavement;
        /// <summary>Lanes each way; the lines between them are dashed.</summary>
        internal readonly int Lanes;
        /// <summary>Width of the planted island down the middle; zero for a painted centre line.</summary>
        internal readonly float Median;

        private RoadProfile(float top, float pavement, int lanes, float median)
        {
            Top = top; Pavement = pavement; Lanes = lanes; Median = median;
        }

        internal static RoadProfile Of(MapWay way)
        {
            switch (way.kind)
            {
                case MapPlan.Ring: return new RoadProfile(.066f, 3, 2, 1.4f);
                case MapPlan.Artery: return new RoadProfile(.062f, 4, 2, 0);
                default: return new RoadProfile(.058f, 2.5f, 1, 0);
            }
        }
    }
}
