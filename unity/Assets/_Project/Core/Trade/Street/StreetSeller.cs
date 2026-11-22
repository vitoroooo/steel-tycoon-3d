using System;
using Steel.Core.Scrap;
using Steel.Core.State.Slice;

namespace Steel.Core.Trade.Street
{
    /// <summary>One door in the city with scrap for sale: what lies there this round and how the talk about it stands.</summary>
    public sealed partial class StreetSeller
    {
        private readonly StreetRules rules;
        private readonly int index;
        private double floor;
        public string Id { get; }
        public SellerKind Kind { get; }
        public WorldPoint Position { get; }
        public KindRules Habits => rules.Of(Kind);
        public long Round { get; private set; } = -1;
        /// <summary>What is still for sale; null once everything went.</summary>
        public ScrapPile Stock { get; private set; }
        /// <summary>The weight the seller guesses for what is left. Paying without weighing pays for this.</summary>
        public double ClaimedKg { get; private set; }
        public bool Weighed { get; private set; }
        /// <summary>Asking price as a fraction of the factory's price for a collector.</summary>
        public double Ask { get; private set; }
        public int Patience { get; private set; }
        public bool Agreed { get; private set; }

        internal StreetSeller(int index, SellerKind kind, WorldPoint position, StreetRules rules)
        {
            this.index = index; Id = "s" + index; Kind = kind; Position = position; this.rules = rules;
        }

        /// <summary>Puts out fresh goods when a new round began; the same round always gives the same goods.</summary>
        internal void Open(long round)
        {
            if (round == Round) return;
            Round = round;
            var k = Habits;
            double kg = Math.Round(k.MinKg + (k.MaxKg - k.MinKg) * Roll(1));
            Stock = new ScrapPile(kg, Roll(2) < k.CommonShare ? k.Common : k.Rare, Math.Round(k.Dirt * Roll(3), 2));
            ClaimedKg = Math.Round(kg * (1 + k.Claim * Math.Max(0, Roll(4) - .4) / .6));
            double drift = 1 + rules.Spread * (Roll(5) * 2 - 1);
            floor = k.Floor * drift; Ask = k.Ask * drift;
            Weighed = Agreed = false; Patience = k.Patience;
        }

        private double Roll(int salt) => SellerRoll.Of(index, Round, salt);

        private ScrapPile Goods => Stock ?? throw new InvalidOperationException("Barangnya sudah habis. Datang lagi nanti.");

        public void Inspect() => Goods.Inspect();

        /// <summary>Puts the goods on the scale: from now on the true weight is paid, not the guess.</summary>
        public void Weigh() { ClaimedKg = Goods.GrossKg; Weighed = true; }

        /// <summary>Kilograms that are charged for taking <paramref name="kg"/> of the goods.</summary>
        public double Charged(double kg) => kg * ClaimedKg / Goods.GrossKg;

        /// <summary>Hands over at most <paramref name="roomKg"/>, already looked at, at what was paid for it.</summary>
        internal ScrapPile Take(double roomKg, double paid)
        {
            var all = Goods;
            if (all.GrossKg <= roomKg) { Stock = null; ClaimedKg = 0; return all.Bought(all.GrossKg, paid); }
            ClaimedKg -= Charged(roomKg);
            Stock = all.Part(all.GrossKg - roomKg);
            return all.Bought(roomKg, paid);
        }
    }
}
