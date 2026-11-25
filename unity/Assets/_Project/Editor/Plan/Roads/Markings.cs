namespace Steel.Editor.Plan.Roads
{
    /// <summary>Paint on the asphalt: a yellow centre line, white dashes between lanes, white edge lines.</summary>
    internal static class Markings
    {
        private const float Lift = .012f, LineWidth = .16f, EdgeInset = .45f;

        internal static void Create(Paving paving, WayLine line, RoadProfile profile, RoadKit kit)
        {
            float half = line.Way.width / 2, height = profile.Top + Lift;
            bool Open(int piece) => !paving.Crossed(line.Middle(piece), line, half + 1);
            Strip yellow = kit.Yellow, white = kit.White;
            if (profile.Median <= 0)
            {
                float gap = profile.Lanes > 1 ? .14f : 0;   // wide streets get a double line
                foreach (float offset in gap > 0 ? new[] { -gap, gap } : new[] { 0f })
                    Line(yellow, line, offset, height, piece => Open(piece) && (gap > 0 || piece % 3 != 2));
            }
            float lane = (half - profile.Median / 2) / profile.Lanes;
            for (int index = 1; index < profile.Lanes; index++)
                foreach (int side in new[] { -1, 1 })
                    Line(white, line, side * (profile.Median / 2 + lane * index), height, piece => Open(piece) && piece % 3 == 0);
            foreach (int side in new[] { -1, 1 })
                Line(white, line, side * (half - EdgeInset), height, Open);
        }

        private static void Line(Strip strip, WayLine line, float offset, float height, System.Func<int, bool> keep) =>
            strip.Lay(line, offset - LineWidth / 2, height, offset + LineWidth / 2, height, keep);
    }
}
