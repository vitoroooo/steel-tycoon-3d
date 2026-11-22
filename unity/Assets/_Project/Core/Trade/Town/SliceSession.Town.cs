using System;
using System.Collections.Generic;
using System.Linq;
using Steel.Core.Scrap;
using Steel.Core.Trade.Town;

namespace Steel.Core.State.Slice
{
    /// <summary>Selling to another player's yard at the price on his board (PRD 5.1 "Jual ke: pemain Kasta 2").
    /// The boards come from the server and are not part of the saved career.</summary>
    public sealed partial class SliceSession
    {
        public PlayerYard[] Yards { get; private set; } = new PlayerYard[0];
        /// <summary>What each of those yards can still pay; only the server knows.</summary>
        private double[] budgets;
        /// <summary>Loads sold since the server last collected them, to be booked on the buyers' careers.</summary>
        public List<Delivery> Delivered { get; } = new List<Delivery>();

        public void PostYards(PlayerYard[] yards, double[] canPay)
        {
            Yards = yards ?? new PlayerYard[0]; budgets = canPay;
        }

        private static bool Takes(PlayerYard yard, ScrapPile pile) =>
            pile.KnownGrade.HasValue && (int)pile.KnownGrade.Value < yard.bids.Length && yard.bids[(int)pile.KnownGrade.Value] > 0;

        /// <summary>What the yard on this plot would pay for the sorted load carried now.</summary>
        public Delivery WeighFor(int plot)
        {
            PlayerYard yard = Yards.FirstOrDefault(y => y.plot == plot) ??
                throw new InvalidOperationException("Kavling ini belum ditempati bandar.");
            if (!Bag.IsSorted) throw new InvalidOperationException("Sortir muatan dahulu di lapak.");
            var note = new Delivery { plot = plot };
            foreach (ScrapPile pile in Bag.Items.Where(pile => Takes(yard, pile)))
            {
                int grade = (int)pile.KnownGrade.Value;
                note.kg[grade] += pile.SaleWeightKg;
                note.paid[grade] += pile.SaleWeightKg * pile.RustFactor * yard.bids[grade];
                note.cost += pile.AcquisitionCost;
            }
            if (note.Kg <= 0) throw new InvalidOperationException("Bandar ini tidak memasang harga untuk muatan Anda.");
            if (note.Kg > yard.roomKg) throw new InvalidOperationException("Lapak bandar ini penuh.");
            return note;
        }

        public double DeliverTo(int plot, WorldPoint post)
        {
            RequireNear(post);
            Delivery note = WeighFor(plot);
            int at = Array.FindIndex(Yards, y => y.plot == plot);
            if (budgets != null && note.Pay > budgets[at])
                throw new InvalidOperationException("Kas bandar ini tidak cukup untuk muatan Anda.");
            CreditProceeds(note.Pay, note.Pay - note.cost);
            CreditSale(note.Kg);
            Bag.RemoveWhere(pile => Takes(Yards[at], pile));
            Yards[at].roomKg -= note.Kg;
            if (budgets != null) budgets[at] -= note.Pay;
            Delivered.Add(note);
            return note.Pay;
        }

        /// <summary>The buyer's side: the load another player sold at this yard's board is paid and stacked.</summary>
        public void TakeDelivery(Delivery note)
        {
            for (int grade = 0; grade < note.kg.Length; grade++)
            {
                if (note.kg[grade] <= 0) continue;
                Cash = Math.Max(0, Cash - note.paid[grade]);
                Yard.stock[grade] += note.kg[grade];
                RecordStockPurchase(grade, note.kg[grade], note.paid[grade]);
                LastReceipt = new Trade.Haul.YardReceipt(LastReceipt.Serial + 1, grade, note.kg[grade], note.paid[grade]);
                if (grade == 0) Yard.exposed.Add(new Saving.SteelBatch { kg = note.kg[grade] });
            }
        }
    }
}
