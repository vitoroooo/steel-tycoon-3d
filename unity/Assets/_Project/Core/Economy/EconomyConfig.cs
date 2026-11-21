using Steel.Core.State;

namespace Steel.Core.Economy
{
    public sealed class EconomyConfig
    {
        public double BaseIbn { get; }
        public double CycleAmplitude { get; }
        public double CycleRealSeconds { get; }
        public double DailyNoiseFraction { get; }
        public int NoiseSeed { get; }
        public double SupplyLimit { get; }
        public double SupplyTargetKg { get; }
        public double ScrapReferenceFraction { get; }
        public double RefreshRealSeconds { get; }
        public double AiBuyK1Fraction { get; }
        public double AiBuyK2Fraction { get; }
        public double NpcSellK1Fraction { get; }
        public double AiSellBandarFraction { get; }
        public double AiSellSmelterFraction { get; }
        public double TransactionTax { get; }

        public EconomyConfig(string json)
        {
            var v = new ConfigValues(json);
            BaseIbn = v.Positive("baseIbn");
            CycleAmplitude = v.Fraction("cycleAmplitude");
            CycleRealSeconds = v.Positive("cycleRealSeconds");
            DailyNoiseFraction = v.Fraction("dailyNoiseFraction");
            NoiseSeed = v.Integer("noiseSeed");
            SupplyLimit = v.Fraction("supplyLimit");
            SupplyTargetKg = v.Positive("supplyTargetKg");
            ScrapReferenceFraction = v.Fraction("scrapReferenceFraction");
            RefreshRealSeconds = v.Positive("refreshRealSeconds");
            AiBuyK1Fraction = v.Positive("aiBuyK1Fraction");
            AiBuyK2Fraction = v.Positive("aiBuyK2Fraction");
            NpcSellK1Fraction = v.Positive("npcSellK1Fraction");
            AiSellBandarFraction = v.Positive("aiSellBandarFraction");
            AiSellSmelterFraction = v.Positive("aiSellSmelterFraction");
            TransactionTax = v.Fraction("transactionTax");
        }
    }
}
