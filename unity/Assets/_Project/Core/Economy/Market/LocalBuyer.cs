using Steel.Core.Scrap;
using Steel.Core.State;
using Steel.Core.State.Slice;

namespace Steel.Core.Economy.Market
{
    public sealed class LocalBuyer
    {
        public string Id { get; }
        public string Name { get; }
        public WorldPoint Position { get; }
        private readonly double fraction, castPremium;
        public bool ReuseOnly => Id == "toko";

        private LocalBuyer(ConfigValues v, string id, string name)
        {
            Id = id; Name = name; Position = new WorldPoint(v.Get(id + "X"), v.Get(id + "Z"));
            fraction = ReuseOnly ? 1 : v.Fraction(id + "Fraction");
            castPremium = ReuseOnly ? 1 : v.Positive(id + "Cast");
        }
        public static LocalBuyer[] Read(ConfigValues v) => new[] {
            new LocalBuyer(v, "pasar", "Pasar Lama · Toko Besi Rejeki"),
            new LocalBuyer(v, "bengkel", "Koridor Bengkel · Bengkel Las"),
            new LocalBuyer(v, "toko", "Kawasan Industri · Toko Bangunan")
        };
        public bool Accepts(ScrapGrade grade) => (grade == ScrapGrade.Reuse) == ReuseOnly;
        public double Rate(ScrapPricing pricing, AiQuote prices, ScrapGrade grade, double quality) =>
            Accepts(grade) ? pricing.UnitRate(grade, quality, prices.ScrapReference) * fraction *
                (grade == ScrapGrade.CastIron ? castPremium : 1) : 0;
    }
}
