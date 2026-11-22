using Steel.Core.State;
using Steel.Core.State.Slice;

namespace Steel.Core.Trade.Haul
{
    /// <summary>Tuning for hauling yard stock in the player's own truck (PRD 5.2, A.1, A.2), read from Haul.json.</summary>
    public sealed class HaulRules
    {
        /// <summary>Where a truck stands to be loaded at the player's yard, and to be weighed at the mill gate.</summary>
        public readonly WorldPoint Yard, Gate;
        public readonly double TruckKg, CraneRate, YardReach, GateReach, DeskReach;
        private readonly ConfigValues values;

        public HaulRules(string json, WorldPoint yard, WorldPoint gate)
        {
            values = new ConfigValues(json); Yard = yard; Gate = gate;
            TruckKg = values.Positive("truckKg"); CraneRate = values.Positive("craneKgPerHour");
            YardReach = values.Positive("yardReach"); GateReach = values.Positive("gateReach");
            DeskReach = values.Positive("deskReach");
        }

        /// <summary>Fuel, toll and driver's meal for one delivery with this truck (PRD A.1 "biaya jalan").</summary>
        public double RunCost(string truck) => values.Positive(truck + "Run");
    }
}
