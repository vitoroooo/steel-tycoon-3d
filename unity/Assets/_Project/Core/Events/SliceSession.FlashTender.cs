using System;
using Steel.Core.Crew;
using Steel.Core.Events;

namespace Steel.Core.State.Slice
{
    /// <summary>"Tender kilat" (PRD appendix E, Kasta 3): a small steel order that must be delivered within one game day.</summary>
    public sealed partial class SliceSession
    {
        private static readonly string[] SteelNames = { "billet", "H-beam", "baut" };
        public static string SteelName(int product) => SteelNames[product];

        private void OfferFlashTender(double hour, double[] rates)
        {
            EventSave e = Events;
            if (e.flash || !Dice.Roll(ref e.sequence, EventTerms.Get("flashChance"))) return;
            bool[] lines = { true, Mill.rolling, Mill.bolts };
            int product = (int)(Dice.Next(ref e.sequence) * 3);
            while (!lines[product]) product = (product + 2) % 3;      // falls back towards billet, which every mill casts
            double low = EventTerms.Get("flashMinKg"), high = EventTerms.Get("flashMaxKg");
            e.flash = true; e.flashTaken = false; e.flashProduct = product; e.flashSent = 0;
            e.flashKg = Math.Round((low + (high - low) * Dice.Next(ref e.sequence)) / 1000) * 1000;
            e.flashPrice = Math.Round(rates[product] * EventTerms.Get("flashPremium"));
            e.flashUntil = hour + EventTerms.Get("flashHours");
            e.note = "TENDER KILAT: " + (e.flashKg / 1000).ToString("0") + " ton " + SteelNames[product] + ", tenggat 1 hari game.";
        }

        public void TakeFlashTender(double hour)
        {
            EventSave e = Events;
            if (!e.flash || e.flashTaken || hour >= e.flashUntil) throw new InvalidOperationException("Tidak ada tender kilat terbuka.");
            e.flashTaken = true; e.note = "Tender kilat diambil. Kirim sebelum tenggat.";
        }

        /// <summary>Sends what the mill has of the product from its gate. The payment for this delivery.</summary>
        public double DeliverFlashTender(double hour)
        {
            EventSave e = Events;
            RequireNear(Rules.Factory);
            if (!e.flashTaken || hour >= e.flashUntil) throw new InvalidOperationException("Tidak ada tender kilat yang sedang berjalan.");
            double kg = Math.Min(e.FlashLeft, Mill.goods[e.flashProduct]);
            if (kg <= 0) throw new InvalidOperationException("Belum ada " + SteelNames[e.flashProduct] + " siap kirim.");
            double payment = kg * e.flashPrice;
            CollectSteelProceeds(e.flashProduct, kg, payment);
            e.flashSent += kg;
            if (e.FlashLeft <= 0) EndFlashTender(EventTerms.Get("flashReputation"), "Tender kilat selesai tepat waktu.");
            return payment;
        }

        /// <summary>An offer nobody took just lapses; one taken and not finished costs reputation like a failed SPK.</summary>
        private void CloseFlashTender(double hour)
        {
            if (!Events.flash || hour < Events.flashUntil) return;
            if (Events.flashTaken) EndFlashTender(-EventTerms.Get("flashFailReputation"), "Tender kilat gagal. Reputasi turun.");
            else { Events.flash = false; Events.note = "Tender kilat lewat tanpa diambil."; }
        }

        private void EndFlashTender(double reputation, string note)
        {
            Yard.reputation = (int)CrewSave.Meter(Yard.reputation + reputation);
            Events.flash = Events.flashTaken = false; Events.note = note;
        }
    }
}
