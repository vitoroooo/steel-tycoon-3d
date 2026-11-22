using System;
using System.Collections.Generic;
using Steel.Core.Scrap;

namespace Steel.Core.State.Slice
{
    public sealed class SliceWorld
    {
        private readonly List<WorldPile> piles = new List<WorldPile>();
        public IReadOnlyList<WorldPile> Piles => piles.AsReadOnly();
        private readonly int supplyHours;
        public SliceWorld(string json)
        {
            var v = new ConfigValues(json);
            supplyHours = v.Integer("supplyHours");
            if (supplyHours == 0) throw new ArgumentOutOfRangeException("supplyHours");
            int count = v.Integer("count");
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            for (int i = 0; i < count; i++)
            {
                string id = "p" + i;
                piles.Add(new WorldPile(id, new WorldPoint(v.Get(id + "x"), v.Get(id + "z")),
                    new ScrapPile(v.Positive(id + "kg"), (ScrapGrade)v.Integer(id + "grade"),
                        v.Optional(id + "dirt", 0), v.Optional(id + "quality", 0))));
            }
        }
        public void OnHour(long hour)
        {
            if (hour > 0 && hour % supplyHours == 0)
                foreach (var pile in piles) pile.Restock();
        }

        public WorldPile Find(string id) => piles.Find(p => p.Id == id) ??
            throw new ArgumentException("Tumpukan tidak ditemukan.");
    }
}
