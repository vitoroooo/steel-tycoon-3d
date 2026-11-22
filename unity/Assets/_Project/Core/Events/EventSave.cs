using System;

namespace Steel.Core.Events
{
    /// <summary>The events waiting for an answer and the ones already rolled for. Saved as is.</summary>
    [Serializable] public sealed class EventSave
    {
        public int sequence;
        /// <summary>The last game day an inspection and a flash tender were rolled for.</summary>
        public long rolledDay = -1;
        /// <summary>A dirty deposit on the scale: its grade, gross weight, share of dirt, and the game hour the collector stops waiting.</summary>
        public int dirtGrade;
        public double dirtKg, dirt, dirtUntil;
        /// <summary>After a load was turned away, collectors bring clean loads until this game hour.</summary>
        public double honestUntil;
        /// <summary>IBN at the start of the current price window, and the last window that opened with a spike and by how much.</summary>
        public long ibnWindow = -1, spikeWindow = -1;
        public double ibnOpen, spike;
        /// <summary>A flash tender: one steel product, a weight, a price per kg and a deadline one game day away.</summary>
        public bool flash, flashTaken;
        public int flashProduct;
        public double flashKg, flashPrice, flashUntil, flashSent;
        public string note;

        public bool DirtyWaiting => dirtKg > 0;
        public double FlashLeft => flashKg - flashSent;

        internal static EventSave Restore(EventSave saved)
        {
            if (saved == null) return new EventSave();
            bool Bad(double value) => !(value >= 0) || double.IsInfinity(value);
            if (saved.sequence < 0 || saved.dirtGrade < 0 || saved.dirtGrade > 3 || saved.dirt >= 1 ||
                saved.flashProduct < 0 || saved.flashProduct > 2 || saved.flashTaken && !saved.flash ||
                saved.flashSent > saved.flashKg || saved.flash && !(saved.flashKg > 0 && saved.flashPrice > 0) ||
                Bad(saved.dirtKg) || Bad(saved.dirt) || Bad(saved.dirtUntil) || Bad(saved.honestUntil) || Bad(saved.ibnOpen) ||
                Bad(saved.spike) || Bad(saved.flashKg) || Bad(saved.flashPrice) || Bad(saved.flashUntil) || Bad(saved.flashSent))
                throw new FormatException("Data kejadian rusak.");
            return saved;
        }
    }
}
