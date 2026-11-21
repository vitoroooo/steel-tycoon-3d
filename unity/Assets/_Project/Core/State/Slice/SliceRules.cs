namespace Steel.Core.State.Slice
{
    public sealed class SliceRules
    {
        public double StartCash { get; }
        public double FirstSaleGrant { get; }
        public double CarryCapKg { get; }
        public double InteractRange { get; }
        public WorldPoint Home { get; }
        public WorldPoint Factory { get; }
        public WorldPoint Spawn { get; }
        public SliceRules(string json)
        {
            var v = new ConfigValues(json);
            StartCash = NumberRules.NonNegative(v.Get("startCash"), "startCash");
            FirstSaleGrant = v.Positive("firstSaleGrant");
            CarryCapKg = v.Positive("carryCapKg");
            InteractRange = v.Positive("interactRange");
            Home = new WorldPoint(v.Get("homeX"), v.Get("homeZ"));
            Factory = new WorldPoint(v.Get("factoryX"), v.Get("factoryZ"));
            Spawn = new WorldPoint(v.Get("spawnX"), v.Get("spawnZ"));
        }
    }
}
