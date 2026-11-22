using System;
using Steel.Core.Scrap;

namespace Steel.Core.State.Slice
{
    public sealed class WorldPile
    {
        private readonly ScrapPile full;
        public string Id { get; }
        public WorldPoint Position { get; }
        /// <summary>What lies at the site now; lighter than the full pile once part of it was carried off.</summary>
        public ScrapPile Scrap { get; private set; }
        public bool Collected { get; private set; }
        /// <summary>Queue number handed out when the site was emptied; the lowest refills first.</summary>
        public long EmptiedOrder { get; private set; }
        public WorldPile(string id, WorldPoint position, ScrapPile scrap)
        { Id = id; Position = position; Scrap = full = scrap; }

        internal void Empty(long order) { Collected = true; EmptiedOrder = order; }

        /// <summary>Hands over at most <paramref name="roomKg"/>; the rest stays and the site is not emptied.</summary>
        internal ScrapPile Take(double roomKg, long order)
        {
            var all = Scrap;
            if (all.GrossKg <= roomKg) { Empty(order); return all; }
            Scrap = all.Part(all.GrossKg - roomKg);
            return all.Part(roomKg);
        }

        /// <summary>Restores how much a saved, partly carried site still holds; 0 means the full pile.</summary>
        internal void Leave(double kg)
        {
            if (kg < 0 || kg > full.GrossKg) throw new FormatException("Sisa tumpukan tidak valid.");
            Scrap = kg > 0 && kg < full.GrossKg ? full.Fresh().Part(kg) : full.Fresh();
        }

        internal void Restock()
        {
            if (!Collected) return;
            Scrap = full.Fresh();
            Collected = false; EmptiedOrder = 0;
        }
    }
}
