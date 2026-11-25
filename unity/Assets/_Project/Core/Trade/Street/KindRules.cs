using System;
using Steel.Core.Scrap;
using Steel.Core.State;

namespace Steel.Core.Trade.Street
{
    /// <summary>How one kind of seller stocks, prices and bargains. Prices are fractions of what the factory pays a collector.</summary>
    public sealed class KindRules
    {
        public double MinKg { get; }
        public double MaxKg { get; }
        /// <summary>The first price asked and the lowest the seller will take.</summary>
        public double Ask { get; }
        public double Floor { get; }
        /// <summary>How far a guessed weight can run over the true one.</summary>
        public double Claim { get; }
        public double Dirt { get; }
        public int Patience { get; }
        public ScrapGrade Common { get; }
        public ScrapGrade Rare { get; }
        public double CommonShare { get; }

        internal KindRules(ConfigValues v, string key)
        {
            MinKg = v.Positive(key + "MinKg"); MaxKg = v.Positive(key + "MaxKg");
            Ask = v.Fraction(key + "Ask"); Floor = v.Fraction(key + "Floor");
            Claim = v.Fraction(key + "Claim"); Dirt = v.Fraction(key + "Dirt");
            Patience = v.Integer(key + "Patience");
            Common = (ScrapGrade)v.Integer(key + "Common"); Rare = (ScrapGrade)v.Integer(key + "Rare");
            CommonShare = v.Fraction(key + "CommonShare");
            if (MaxKg < MinKg || Floor <= 0 || Ask <= Floor || Patience < 1)
                throw new FormatException("Aturan penjual tidak valid: " + key);
        }
    }
}
