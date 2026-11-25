using System.Collections.Generic;
using System.Linq;
using Steel.Core.Trade.Town;

namespace Steel.Career.City
{
    /// <summary>Every yard that is buying (PRD 5.2 Papan Harga Beli). A player sees the others' yards on the city's
    /// numbered plots; plot 1 is always his own. Used only inside the career book's lock.</summary>
    public sealed class TownYards
    {
        /// <summary>The city has six yard plots, so a player sees five other yards.</summary>
        public const int FirstPlot = 2, Shown = 5;
        private sealed class Posted { public PlayerYard Yard; public double CanPay; }
        private readonly SortedDictionary<long, Posted> boards = new SortedDictionary<long, Posted>();

        public void Post(long account, string name, Careers.Career career)
        {
            PlayerYard yard = career.Board(out double canPay);
            if (yard == null) { boards.Remove(account); return; }
            yard.owner = name;
            boards[account] = new Posted { Yard = yard, CanPay = canPay };
        }

        public void Drop(long account) => boards.Remove(account);
        public void Clear() => boards.Clear();

        /// <summary>The accounts whose yards this player sees, in plot order.</summary>
        public long[] Neighbours(long viewer) => boards.Keys.Where(id => id != viewer).Take(Shown).ToArray();

        public PlayerYard[] For(long viewer, out double[] canPay)
        {
            Posted[] seen = Neighbours(viewer).Select(id => boards[id]).ToArray();
            canPay = seen.Select(posted => posted.CanPay).ToArray();
            return seen.Select((posted, i) => new PlayerYard
            {
                owner = posted.Yard.owner, plot = FirstPlot + i, bids = posted.Yard.bids.ToArray(), roomKg = posted.Yard.roomKg
            }).ToArray();
        }
    }
}
