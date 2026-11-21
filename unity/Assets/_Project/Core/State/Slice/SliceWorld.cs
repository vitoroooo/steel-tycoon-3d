using System;
using System.Collections.Generic;
using Steel.Core.Scrap;

namespace Steel.Core.State.Slice
{
    public sealed class SliceWorld
    {
        private readonly List<WorldPile> piles = new List<WorldPile>();
        public IReadOnlyList<WorldPile> Piles => piles.AsReadOnly();
        public SliceWorld(string json)
        {
            var v = new ConfigValues(json);
            int count = v.Integer("count");
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            for (int i = 0; i < count; i++)
            {
                string id = "p" + i;
                piles.Add(new WorldPile(id, new WorldPoint(v.Get(id + "x"), v.Get(id + "z")),
                    new ScrapPile(v.Positive(id + "kg"), (ScrapGrade)v.Integer(id + "grade"))));
            }
        }
        public WorldPile Find(string id) => piles.Find(p => p.Id == id) ??
            throw new ArgumentException("Tumpukan tidak ditemukan.");
    }
}
