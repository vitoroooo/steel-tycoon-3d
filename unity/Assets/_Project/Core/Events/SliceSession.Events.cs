using System;
using Steel.Core.Events;
using Steel.Core.Trade.Spk;

namespace Steel.Core.State.Slice
{
    /// <summary>The events of PRD appendix E without a home elsewhere: price spikes, environmental inspections, flash tenders.
    /// Dirty deposits and the flash tender's delivery are in their own files.</summary>
    public sealed partial class SliceSession
    {
        public EventRules EventTerms { get; private set; }
        public EventSave Events { get; private set; } = new EventSave();
        public void OpenEvents(EventRules rules) => EventTerms = rules ?? throw new ArgumentNullException(nameof(rules));
        public void RestoreEvents(EventSave saved) => Events = EventSave.Restore(saved);

        /// <summary>Once every game hour, with today's IBN and what a kg of each steel product sells for.</summary>
        public void AdvanceEvents(double hour, double ibn, double[] steelRates)
        {
            WatchIbn(hour, ibn);
            CloseFlashTender(hour);
            long day = SpkRules.Day(hour);
            if (!Mill.opened || day <= Events.rolledDay) return;
            Events.rolledDay = day;
            Inspect(hour);
            OfferFlashTender(hour, steelRates);
        }

        /// <summary>"Lonjakan harga": IBN opens a window more than 8% above the one before. A chance to sell, never a cost.</summary>
        private void WatchIbn(double hour, double ibn)
        {
            long window = (long)Math.Floor(hour / EventTerms.Get("spikeWindowHours"));
            if (window <= Events.ibnWindow) return;
            double rise = Events.ibnOpen > 0 && window == Events.ibnWindow + 1 ? ibn / Events.ibnOpen - 1 : 0;
            if (rise > EventTerms.Get("spikeRise"))
            {
                Events.spikeWindow = window; Events.spike = rise;
                Events.note = "LONJAKAN HARGA: IBN naik " + (rise * 100).ToString("0.#") + "%. Kesempatan jual.";
            }
            Events.ibnOpen = ibn; Events.ibnWindow = window;
        }
        public bool PriceSpike(double hour) =>
            Events.spikeWindow >= 0 && Events.spikeWindow == (long)Math.Floor(hour / EventTerms.Get("spikeWindowHours"));

        /// <summary>"Inspeksi lingkungan" (Kasta 3): random, twice as often without the enclosed shredder. A mill whose permits are
        /// complete passes; one without an active K3 certificate is fined, unless the player is still protected from bad events.</summary>
        private void Inspect(double hour)
        {
            double chance = EventTerms.Get("inspectChance") * (Mill.shredder ? 1 : EventTerms.Get("openShredderFactor"));
            if (!Dice.Roll(ref Events.sequence, chance)) return;
            if (Mill.safetyReady > 0 && hour >= Mill.safetyReady) { Events.note = "Inspeksi lingkungan: izin lengkap, lulus."; return; }
            if (!EventAvailable(hour)) return;
            RecordCrewEvent(hour);
            double fine = EventTerms.Get("inspectFine");
            ChargeOperatingExpense(fine);
            Events.note = "Inspeksi lingkungan: sertifikat K3 belum aktif. Denda " + fine.ToString("N0") + ".";
        }
    }
}
