namespace Steel.Core.Economy
{
    public sealed class AiQuote
    {
        public double RefreshedAtRealSeconds { get; }
        public double Ibn { get; }
        public double ScrapReference { get; }
        public double BuyFromK1 { get; }
        public double BuyFromK2 { get; }
        public double SellToK1 { get; }
        public double SellToBandar { get; }
        public double SellToSmelter { get; }
        public double BuyIngot => Ibn;

        internal AiQuote(double time, double ibn, EconomyConfig config)
        {
            RefreshedAtRealSeconds = time;
            Ibn = ibn;
            ScrapReference = ibn * config.ScrapReferenceFraction;
            BuyFromK1 = ScrapReference * config.AiBuyK1Fraction;
            BuyFromK2 = ScrapReference * config.AiBuyK2Fraction;
            SellToK1 = ScrapReference * config.NpcSellK1Fraction;
            SellToBandar = ScrapReference * config.AiSellBandarFraction;
            SellToSmelter = ScrapReference * config.AiSellSmelterFraction;
        }
    }
}
