using System;
using Steel.Core.Trade.Haul;

namespace Steel.Core.State.Slice
{
    /// <summary>Delivering a truck load at the mill gate: weighed on the bridge, emptied by the crane, then paid.</summary>
    public sealed partial class SliceSession
    {
        /// <summary>What the mill would pay for the load at this price per kg, after the truck's running cost.</summary>
        public double GateNet(TruckLoad load, double perKg) => load.billableKg * perKg - Haul.RunCost(load.truck);

        public double WeighIn(string truck, double perKg, double hour)
        {
            var load = LoadOn(truck) ?? throw new InvalidOperationException("Bak truk kosong.");
            if (load.Loading) throw new InvalidOperationException("Loader masih memuat.");
            if (load.Unloading) throw new InvalidOperationException("Truk ini sedang dibongkar.");
            if (Position.Distance(Haul.Gate) > Haul.DeskReach) throw new InvalidOperationException("Datangi pos timbang pabrik.");
            if (!Parked(Fleet.Find(truck), Haul.Gate, Haul.GateReach))
                throw new InvalidOperationException("Parkir truk di jembatan timbang dahulu.");
            double net = GateNet(load, NumberRules.NonNegative(perKg, nameof(perKg)));
            if (net <= 0) throw new InvalidOperationException("Nilai muatan tidak menutup biaya jalan.");
            load.payout = net; load.weighedKg = load.kg; load.doneHour = hour + load.kg / Haul.CraneRate;
            UseAsset(truck);
            return net;
        }

        private void PayLoad(TruckLoad load)
        {
            double share = load.kg > 0 ? Math.Min(1, load.knownKg / load.kg) : 0;
            CreditProceeds(load.payout, load.payout * share - load.cost);
            CreditSale(load.kg);
            Loads.Remove(load);
        }

        /// <summary>Kilograms still on the bed while the mill's crane works, for the load shown on the truck.</summary>
        public double OnBed(TruckLoad load, double hour)
        {
            if (!load.Unloading) return load.kg;
            double span = load.kg / Haul.CraneRate;
            return span <= 0 ? 0 : load.kg * Math.Max(0, Math.Min(1, (load.doneHour - hour) / span));
        }

        internal TruckLoad[] SaveLoads() => Loads.ToArray();

        internal void RestoreLoads(TruckLoad[] saved)
        {
            Loads.Clear();
            if (saved == null) return;
            foreach (var load in saved)
            {
                load.Check();
                if (System.Linq.Enumerable.All(Fleet.All, motor => motor.Rules.Id != load.truck) || LoadOn(load.truck) != null) throw new FormatException("Muatan truk rusak.");
                Loads.Add(load);
            }
        }
    }
}
