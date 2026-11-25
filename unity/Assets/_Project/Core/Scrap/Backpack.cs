using System;
using System.Collections.Generic;
using System.Linq;
using Steel.Core.State.Saving;

namespace Steel.Core.Scrap
{
    public sealed class Backpack
    {
        private readonly List<ScrapPile> items = new List<ScrapPile>();
        public IReadOnlyList<ScrapPile> Items => items.AsReadOnly();
        public double CapacityKg { get; }
        public double CarriedKg { get; private set; }
        public int Revision { get; private set; }
        public bool IsSorted => items.Count > 0 && items.All(p => p.IsSorted);
        public Backpack(double capacityKg)
        {
            CapacityKg = State.NumberRules.NonNegative(capacityKg, nameof(capacityKg));
            if (capacityKg == 0) throw new ArgumentOutOfRangeException(nameof(capacityKg));
        }
        public void Add(ScrapPile pile)
        {
            if (pile == null) throw new ArgumentNullException(nameof(pile));
            if (items.Contains(pile)) throw new InvalidOperationException("Tumpukan sudah dibawa.");
            if (!pile.KnownGrade.HasValue) throw new InvalidOperationException("Periksa grade terlebih dahulu.");
            if (CarriedKg + pile.GrossKg > CapacityKg)
                throw new InvalidOperationException("Muatan penuh. Sortir dan jual dahulu.");
            items.Add(pile); CarriedKg += pile.GrossKg; Revision++;
        }
        public void Sort()
        {
            if (items.Count == 0) throw new InvalidOperationException("Belum ada rongsok untuk disortir.");
            foreach (var pile in items) pile.Sort();
            Revision++;
        }
        public void Clear() { items.Clear(); CarriedKg = 0; Revision++; }
        internal ScrapPile NextUnsorted => items.Find(p => !p.IsSorted);
        internal void SortNext() { NextUnsorted.Sort(); Revision++; }
        internal void RemoveWhere(Predicate<ScrapPile> accepted)
        {
            items.RemoveAll(accepted);
            CarriedKg = items.Sum(p => p.GrossKg);
            Revision++;
        }
        internal LotSave[] Snapshot() => items.Select(p => p.Snapshot()).ToArray();
        internal void Load(LotSave[] lots)
        {
            Clear();
            foreach (var p in lots) Add(ScrapPile.Load(p));
        }
    }
}
