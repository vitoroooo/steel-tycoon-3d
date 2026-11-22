using System;
using Steel.Core.Events;
using Steel.Core.Trade.Spk;

namespace Steel.Core.State.Slice
{
    /// <summary>"Temuan barang reuse" at a Kasta 2 yard: sorting a collector's load turns up to 2 tonnes of usable goods.
    /// Sell them to Toko Konstruksi now, or keep them for a special SPK that pays the top of the band.</summary>
    public sealed partial class SliceSession
    {
        /// <summary>Called after a collector's load was weighed in: the find comes out of the grade it was bought as.</summary>
        public void SortDeposit(int grade, double receivedKg, long hour)
        {
            EventSave e = Events;
            if (!Yard.bandar || receivedKg <= 0 || e.ReuseWaiting || !Dice.Roll(ref e.sequence, EventTerms.Get("reuseChance"))) return;
            double low = EventTerms.Get("reuseMinKg"), high = EventTerms.Get("reuseMaxKg");
            double kg = Math.Min(Math.Min(receivedKg, Yard.stock[grade]), low + (high - low) * Dice.Next(ref e.sequence));
            e.reuseQuality = Dice.Next(ref e.sequence);
            Yard.stock[grade] -= kg; e.reuseKg = kg; e.reuseUntil = hour + EventTerms.Get("reuseAnswerHours");
            e.note = "Temuan barang reuse saat sortir: " + kg.ToString("N0") + " kg layak pakai.";
        }

        /// <summary>What Toko Konstruksi pays per kg for the find, as a multiple of the scrap reference (PRD 6.1: 160-250%).</summary>
        public double ReuseFindRate => EventTerms.Get("reuseLow") +
            (EventTerms.Get("reuseHigh") - EventTerms.Get("reuseLow")) * Events.reuseQuality;

        public double SellReuseFind(double reference)
        {
            if (!Events.ReuseWaiting) throw new InvalidOperationException("Tidak ada temuan reuse yang menunggu.");
            return SellFind(reference, "Temuan reuse dijual ke Toko Konstruksi.");
        }

        public void KeepReuseFind()
        {
            if (!Events.ReuseWaiting) throw new InvalidOperationException("Tidak ada temuan reuse yang menunggu.");
            Events.reuseStore += Events.reuseKg; Events.reuseKg = 0;
            Events.note = "Temuan reuse disimpan untuk SPK khusus.";
        }

        /// <summary>Sells everything kept to Toko Konstruksi at the bottom of the band, so kept goods are never stuck.</summary>
        public double SellReuseStore(double reference)
        {
            if (Events.reuseStore <= 0) throw new InvalidOperationException("Tidak ada barang reuse yang disimpan.");
            double pay = Events.reuseStore * reference * EventTerms.Get("reuseLow");
            Events.reuseStore = 0; Events.reuseOrder = false;
            CreditProceeds(pay, pay);
            Events.note = "Simpanan reuse dijual ke Toko Konstruksi.";
            return pay;
        }

        public double DeliverReuseOrder(double hour)
        {
            EventSave e = Events;
            if (!e.reuseOrder || hour >= e.reuseOrderUntil) throw new InvalidOperationException("Tidak ada SPK khusus reuse yang berlaku.");
            double kg = Math.Min(e.reuseStore, e.reuseOrderKg), pay = kg * e.reuseOrderPrice;
            e.reuseStore -= kg; e.reuseOrder = false;
            CreditProceeds(pay, pay);
            e.note = "SPK khusus reuse terkirim: " + kg.ToString("N0") + " kg.";
            return pay;
        }

        /// <summary>Once every game hour: an unanswered find sells itself, a stale order lapses, and kept goods may draw a special SPK.</summary>
        public void ReuseHour(long hour, double reference)
        {
            EventSave e = Events;
            if (e.ReuseWaiting && hour >= e.reuseUntil) SellFind(reference, "Temuan reuse dijual otomatis ke Toko Konstruksi.");
            if (e.reuseOrder && hour >= e.reuseOrderUntil) e.reuseOrder = false;
            long day = SpkRules.Day(hour);
            if (day <= e.reuseDay) return;
            e.reuseDay = day;
            if (e.reuseStore <= 0 || e.reuseOrder || !Dice.Roll(ref e.sequence, EventTerms.Get("reuseOrderChance"))) return;
            e.reuseOrder = true; e.reuseOrderKg = e.reuseStore;
            e.reuseOrderPrice = reference * EventTerms.Get("reuseHigh");
            e.reuseOrderUntil = hour + EventTerms.Get("reuseOrderHours");
            e.note = "SPK khusus reuse: " + e.reuseOrderKg.ToString("N0") + " kg dengan harga tertinggi.";
        }

        private double SellFind(double reference, string note)
        {
            double pay = Events.reuseKg * reference * ReuseFindRate;
            Events.reuseKg = 0; Events.note = note;
            CreditProceeds(pay, pay);
            return pay;
        }
    }
}
