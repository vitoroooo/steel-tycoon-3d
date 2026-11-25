using System;
using System.Collections.Generic;
using System.Linq;
using Steel.Core.State;
using Steel.Core.State.Saving;
using Steel.Core.State.Slice;

namespace Steel.Core.Trade.Street
{
    /// <summary>Every door in the city that sells scrap. Sellers.json lists where they stand; the map builder writes it.</summary>
    public sealed class StreetBlock
    {
        private readonly List<StreetSeller> sellers = new List<StreetSeller>();
        public StreetRules Rules { get; }
        public IReadOnlyList<StreetSeller> Sellers => sellers.AsReadOnly();

        public StreetBlock(string rulesJson, string sellersJson)
        {
            Rules = new StreetRules(rulesJson);
            var v = new ConfigValues(sellersJson);
            int count = v.Integer("count");
            for (int i = 0; i < count; i++)
            {
                int kind = v.Integer("s" + i + "kind");
                if (!Enum.IsDefined(typeof(SellerKind), kind)) throw new FormatException("Jenis penjual tidak dikenal: s" + i);
                sellers.Add(new StreetSeller(i, (SellerKind)kind, new WorldPoint(v.Get("s" + i + "x"), v.Get("s" + i + "z")), Rules));
            }
            Advance(0);
        }

        /// <summary>Every door puts out new goods once per restock round, whatever was left of the old ones.</summary>
        public void Advance(double gameHours)
        {
            long round = (long)Math.Floor(gameHours / Rules.RestockHours);
            foreach (var seller in sellers) seller.Open(round);
        }

        public double HoursToRestock(double gameHours) => Rules.RestockHours - gameHours % Rules.RestockHours;

        public StreetSeller Find(string id) => sellers.Find(s => s.Id == id) ??
            throw new ArgumentException("Penjual tidak ditemukan.");

        /// <summary>The closest door within reach that still has goods.</summary>
        public StreetSeller Near(WorldPoint point, double range) => sellers
            .Where(s => s.Stock != null && s.Position.Distance(point) <= range)
            .OrderBy(s => s.Position.Distance(point)).FirstOrDefault();

        internal SellerSave[] Snapshot() => sellers.Select(s => s.Snapshot()).ToArray();

        /// <summary>A career saved before doors existed, or before a door was added to the map, simply finds them freshly stocked.</summary>
        internal void Load(SellerSave[] saved)
        {
            foreach (var s in saved ?? Array.Empty<SellerSave>())
                sellers.Find(seller => seller.Id == s.id)?.Load(s);
        }
    }
}
