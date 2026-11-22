using System;
using System.Collections.Generic;
using System.Linq;
using Steel.Core.Scrap;

namespace Steel.Core.State.Slice
{
    /// <summary>Street scrap sites. Residents refill one emptied site every RestockHours game hours.</summary>
    public sealed class SliceWorld
    {
        private readonly List<WorldPile> piles = new List<WorldPile>();
        public IReadOnlyList<WorldPile> Piles => piles.AsReadOnly();
        public double RestockHours { get; }
        private long emptied, refills;
        public SliceWorld(string json)
        {
            var v = new ConfigValues(json);
            RestockHours = v.Positive("restockHours");
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

        public int Available => piles.Count(p => !p.Collected);

        /// <summary>Game hours from the given clock reading until the next site is refilled.</summary>
        public double HoursToRestock(double gameHours) => RestockHours - gameHours % RestockHours;

        /// <summary>Carries off what fits in <paramref name="roomKg"/>; a site only joins the refill queue once empty.</summary>
        internal ScrapPile Take(WorldPile pile, double roomKg)
        {
            var taken = pile.Take(roomKg, emptied + 1);
            if (pile.Collected) emptied++;
            return taken;
        }

        /// <summary>Restores a saved site; sites emptied before the queue existed keep their listed order.</summary>
        internal void Restore(WorldPile pile, bool collected, long order, double leftKg)
        {
            if (order < 0) throw new FormatException("Urutan tumpukan tidak valid.");
            if (collected) pile.Empty(order); else { pile.Restock(); pile.Leave(leftKg); }
            emptied = Math.Max(emptied, order);
        }

        /// <summary>Starts counting refills from this clock reading, so a restored career is not refilled at once.</summary>
        public void Sync(double gameHours) => refills = (long)Math.Floor(gameHours / RestockHours);

        /// <summary>Refills the longest-empty site once for every refill time passed since the last call.</summary>
        public void Advance(double gameHours)
        {
            long due = (long)Math.Floor(gameHours / RestockHours);
            for (long n = Math.Min(due - refills, piles.Count); n > 0; n--)
                piles.Where(p => p.Collected).OrderBy(p => p.EmptiedOrder).FirstOrDefault()?.Restock();
            refills = due;
        }

        public WorldPile Find(string id) => piles.Find(p => p.Id == id) ??
            throw new ArgumentException("Tumpukan tidak ditemukan.");
    }
}
