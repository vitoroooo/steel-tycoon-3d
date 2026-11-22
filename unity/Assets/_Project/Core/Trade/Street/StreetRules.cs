using System;
using Steel.Core.State;

namespace Steel.Core.Trade.Street
{
    /// <summary>Door-to-door buying rules, read from StreetTrade.json.</summary>
    public sealed class StreetRules
    {
        private static readonly string[] Keys = { "house", "workshop", "project", "market" };
        private readonly KindRules[] kinds = new KindRules[Keys.Length];
        /// <summary>Game hours between two lots of goods at the same door.</summary>
        public double RestockHours { get; }
        /// <summary>Share of the gap to the floor a seller gives up after a fair offer that was still too low.</summary>
        public double Concede { get; }
        /// <summary>An offer under this share of the asking price offends: the price stays and patience runs out faster.</summary>
        public double Insult { get; }
        /// <summary>How far one seller's prices drift from the kind's usual ones.</summary>
        public double Spread { get; }
        /// <summary>Rupiah per kilogram one tap on the offer buttons moves.</summary>
        public double Step { get; }
        /// <summary>Metres from a door within which a buyer, or the truck he sits in, can deal.</summary>
        public double Reach { get; }

        public StreetRules(string json)
        {
            var v = new ConfigValues(json);
            RestockHours = v.Positive("restockHours"); Concede = v.Fraction("concede");
            Insult = v.Fraction("insult"); Spread = v.Fraction("spread"); Step = v.Positive("step");
            Reach = v.Positive("reach");
            if (Concede <= 0) throw new FormatException("concede harus di atas nol.");
            for (int i = 0; i < Keys.Length; i++) kinds[i] = new KindRules(v, Keys[i]);
        }

        public KindRules Of(SellerKind kind) => kinds[(int)kind];
    }
}
