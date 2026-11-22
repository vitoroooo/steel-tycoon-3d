namespace Steel.Editor.Plan.Roads
{
    /// <summary>The planted island down the middle of the ring road; it opens wherever another street joins.</summary>
    internal static class Median
    {
        private const float Height = .2f, Opening = 7;

        internal static void Create(Paving paving, WayLine line, RoadProfile profile, RoadKit kit)
        {
            float half = profile.Median / 2, top = profile.Top + Height;
            bool Stands(int piece) => !paving.Crossed(line.Middle(piece), line, Opening);
            Strip grass = kit.Grass, kerb = kit.Concrete;
            grass.Lay(line, -half + RoadBuilder.KerbWidth, top, half - RoadBuilder.KerbWidth, top, Stands);
            foreach (int side in new[] { -1, 1 })
            {
                float edge = side * half, lip = edge - side * RoadBuilder.KerbWidth;
                if (side > 0)
                {
                    kerb.Lay(line, lip, top, edge, top, Stands);
                    kerb.Lay(line, edge, top, edge, profile.Top, Stands);
                }
                else
                {
                    kerb.Lay(line, edge, top, lip, top, Stands);
                    kerb.Lay(line, edge, profile.Top, edge, top, Stands);
                }
            }
        }
    }
}
