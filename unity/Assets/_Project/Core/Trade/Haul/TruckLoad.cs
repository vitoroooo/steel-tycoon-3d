using System;

namespace Steel.Core.Trade.Haul
{
    /// <summary>Sorted yard stock on one of the player's trucks, from the loader's first scoop to the mill's payment. Saved as is.</summary>
    [Serializable] public sealed class TruckLoad
    {
        public string truck;
        public int grade;
        /// <summary>On the bed now; what the mill will pay for after rust; what it cost; how much of it has a known cost.</summary>
        public double kg, billableKg, cost, knownKg;
        /// <summary>The loader keeps scooping until the bed holds this much. Game hour of its last scoop.</summary>
        public double targetKg, sinceHour;
        /// <summary>Set at the weighbridge: what the mill pays once its crane has emptied the bed, and when.</summary>
        public double payout, weighedKg, doneHour;

        public bool Loading => targetKg > kg;
        public bool Unloading => payout > 0;

        internal void Check()
        {
            if (string.IsNullOrEmpty(truck) || grade < 0 || grade > 3 || !(kg >= 0) || !(billableKg >= 0) || !(cost >= 0) ||
                !(knownKg >= 0) || !(targetKg >= 0) || !(payout >= 0) || !(weighedKg >= 0) || billableKg > kg + .001)
                throw new FormatException("Muatan truk rusak.");
        }
    }
}
