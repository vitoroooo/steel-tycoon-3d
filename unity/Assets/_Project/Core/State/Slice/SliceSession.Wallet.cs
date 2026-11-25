using System;

namespace Steel.Core.State.Slice
{
    /// <summary>The one purse shared with the market between players: its cash is this cash, its HMS 1 is the yard's.</summary>
    public sealed partial class SliceSession
    {
        public double MarketStockKg => Yard.stock[0];

        /// <summary>What lots, contracts and association fees moved on the market since the career last looked.</summary>
        public void SettleMarket(double cash, double kg)
        {
            if (double.IsNaN(cash + kg) || double.IsInfinity(cash + kg)) throw new ArgumentOutOfRangeException(nameof(cash));
            Cash = Math.Max(0, Cash + cash);
            if (kg > 0) { Yard.stock[0] += kg; Yard.exposed.Add(new Saving.SteelBatch { kg = kg }); }
            else if (kg < 0)
            {
                double taken = Math.Min(Yard.stock[0], -kg);
                Yard.InvoiceWeight(0, taken, true); Yard.stock[0] -= taken;
            }
        }
    }
}
