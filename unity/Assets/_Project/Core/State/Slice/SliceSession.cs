using System;
using System.Linq;
using Steel.Core.Economy;
using Steel.Core.Scrap;
using Steel.Core.Fleet;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public SliceRules Rules { get; }
        public SliceWorld World { get; }
        public WorldPoint Position { get; set; }
        public double Cash { get; private set; }
        private readonly ScrapPricing pricing;
        public SliceSession(SliceRules rules, SliceWorld world, ScrapPricing pricing, PickupRules pickupRules)
        {
            Rules = rules; World = world; this.pricing = pricing;
            backpack = new Backpack(rules.CarryCapKg); Pickup = new PickupState(pickupRules);
            Cash = rules.StartCash; Position = rules.Spawn;
        }
        private void RequireNear(WorldPoint point)
        {
            if (Position.Distance(point) > Rules.InteractRange)
                throw new InvalidOperationException("Dekati lokasi interaksi terlebih dahulu.");
        }
        private WorldPile Available(string id)
        {
            var pile = World.Find(id);
            if (pile.Collected) throw new InvalidOperationException("Tumpukan sudah dipungut.");
            RequireNear(pile.Position); return pile;
        }
        public void Inspect(string id) => Available(id).Scrap.Inspect();
        public void PickUp(string id)
        {
            var pile = Available(id); Bag.Add(pile.Scrap); pile.Collected = true;
        }
        public void SortAtHome() { RequireNear(Rules.Home); Bag.Sort(); }
        public SaleOffer Preview(AiQuote prices)
        {
            RequireNear(Rules.Factory);
            if (!Bag.IsSorted) throw new InvalidOperationException("Sortir muatan di lapak dahulu.");
            double revenue = Bag.Items.Sum(p => pricing.Quote(p,
                p.KnownGrade == ScrapGrade.Reuse ? prices.ScrapReference : prices.BuyFromK1));
            return new SaleOffer(Bag.Items.Sum(p => p.SaleWeightKg), revenue,
                prices.RefreshedAtRealSeconds, Bag.Revision);
        }
        public double Confirm(SaleOffer offer, AiQuote prices)
        {
            if (offer == null) throw new InvalidOperationException("Lihat penawaran sebelum menjual.");
            var current = Preview(prices);
            if (offer.InventoryRevision != current.InventoryRevision || offer.QuoteTime != current.QuoteTime)
                throw new InvalidOperationException("Penawaran berubah. Lihat harga terbaru dahulu.");
            Cash += current.Revenue; Bag.Clear(); return current.Revenue;
        }
    }
}
