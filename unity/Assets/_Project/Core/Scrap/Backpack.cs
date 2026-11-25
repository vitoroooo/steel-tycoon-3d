using System;
using System.Collections.Generic;
using System.Linq;

namespace Steel.Core.Scrap
{
    public sealed class Backpack
    {
        private readonly List<ScrapPile> items = new List<ScrapPile>();
        public IReadOnlyList<ScrapPile> Items => items.AsReadOnly();
        public double CapacityKg { get; }
        public double CarriedKg => items.Sum(p => p.GrossKg);
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
            items.Add(pile); Revision++;
        }
        public void Sort()
        {
            if (items.Count == 0) throw new InvalidOperationException("Belum ada rongsok untuk disortir.");
            foreach (var pile in items) pile.Sort();
            Revision++;
        }
        public void Clear() { items.Clear(); Revision++; }
    }
}
