using System;
using System.Collections.Generic;
using System.Linq;
using Steel.Core.Equipment;
using Steel.Core.Fleet;
using Steel.Core.Trade.Haul;

namespace Steel.Core.State.Slice
{
    /// <summary>Loading the player's own truck at the yard: the loader moves sorted stock onto the bed as game time passes.</summary>
    public sealed partial class SliceSession
    {
        public HaulRules Haul { get; private set; }
        public List<TruckLoad> Loads { get; private set; } = new List<TruckLoad>();
        /// <summary>Why the loader last stopped early, for the yard desk to show.</summary>
        public string LoaderStopped { get; private set; }

        public void OpenHaul(HaulRules rules) => Haul = rules ?? throw new ArgumentNullException(nameof(rules));
        public TruckLoad LoadOn(string truck) => Loads.Find(load => load.truck == truck);
        private static bool Parked(MotorState motor, WorldPoint dock, double reach) =>
            motor.Owned && motor.Speed == 0 && motor.Position.Distance(dock) <= reach;

        /// <summary>The player's truck standing at a dock, heaviest first, or null.</summary>
        public MotorState TruckAt(WorldPoint dock, double reach) => Fleet.All
            .Where(motor => motor.Rules.CapacityKg >= Haul.TruckKg && Parked(motor, dock, reach))
            .OrderByDescending(motor => motor.Rules.CapacityKg).FirstOrDefault();

        public void StartLoading(string truck, int grade, double hour)
        {
            if (Position.Distance(Haul.Yard) > Haul.DeskReach) throw new InvalidOperationException("Datangi pos muat di kavling Anda.");
            if (!HasLoader) throw new InvalidOperationException("Perlu lapak Kasta 2 dengan loader.");
            var motor = Fleet.Find(truck);
            if (motor.Rules.CapacityKg < Haul.TruckKg) throw new InvalidOperationException("Loader hanya memuat truk engkel ke atas.");
            if (!Parked(motor, Haul.Yard, Haul.YardReach)) throw new InvalidOperationException("Parkir truk Anda di kavling dahulu.");
            if (Loads.Any(other => other.Loading)) throw new InvalidOperationException("Loader masih memuat.");
            var load = LoadOn(truck);
            if (load != null && (load.Unloading || load.grade != grade && load.kg > 0))
                throw new InvalidOperationException("Bak masih berisi muatan lain.");
            double room = motor.Rules.CapacityKg - motor.Cargo.CarriedKg - (load?.kg ?? 0);
            double want = Math.Min(room, Yard.stock[grade]);
            if (want <= 0) throw new InvalidOperationException(room <= 0 ? "Bak sudah penuh." : "Stok grade ini kosong.");
            if (load == null) Loads.Add(load = new TruckLoad { truck = truck });
            load.grade = grade; load.targetKg = load.kg + want; load.sinceHour = hour; LoaderStopped = null;
        }

        public void StopLoading(string truck)
        {
            var load = LoadOn(truck);
            if (load != null) Settle(load);
        }

        private void Settle(TruckLoad load)
        {
            load.targetKg = load.kg;
            if (load.kg <= 0 && !load.Unloading) Loads.Remove(load);
        }

        /// <summary>One stretch of loader work. A truck that drove off, a broken loader or an empty heap ends the job with what is on the bed.</summary>
        private void Scoop(TruckLoad load, double hour)
        {
            double hours = hour - load.sinceHour;
            if (!Parked(Fleet.Find(load.truck), Haul.Yard, Haul.YardReach)) { Settle(load); return; }
            if (hours <= 0) return;
            double kg = Math.Min(Math.Min(load.targetKg - load.kg, Yard.stock[load.grade]), Maintenance.LoaderRate * hours);
            load.sinceHour = hour;
            if (kg <= 0) { Settle(load); return; }
            if (!HasLoader || UseMachine(true, hour, kg / Maintenance.LoaderRate, 0) != WorkResult.Completed)
            {
                LoaderStopped = "Loader berhenti. Periksa halaman Perawatan alat."; Settle(load); return;
            }
            var (cost, known) = IssueStock(load.grade, kg);
            load.billableKg += Yard.InvoiceWeight(load.grade, kg, true);
            Yard.stock[load.grade] -= kg; load.kg += kg; load.cost += cost; load.knownKg += known;
            if (load.kg >= load.targetKg - 1e-6 || Yard.stock[load.grade] <= 0) Settle(load);
        }

        public void AdvanceHaul(double hour)
        {
            foreach (var load in Loads.ToArray())
            {
                if (!Fleet.Find(load.truck).Owned) { Loads.Remove(load); continue; }   // repossessed with its cargo
                if (load.Loading) Scoop(load, hour);
                if (load.Unloading && hour >= load.doneHour) PayLoad(load);
            }
        }
    }
}
