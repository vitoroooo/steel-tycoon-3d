using System;
using System.Linq;
using Steel.Core.Crew;
using Steel.Core.Trade.Haul;
using Steel.Core.Trade.Spk;

namespace Steel.Core.State.Slice
{
    /// <summary>Work orders from the mill: a bond is locked, deliveries at its gate count at the fixed price, late costs 2% an hour.</summary>
    public sealed partial class SliceSession
    {
        public SpkRules SpkTerms { get; private set; }
        public SpkSave Spk { get; private set; } = new SpkSave();

        public void OpenSpk(SpkRules rules) => SpkTerms = rules ?? throw new ArgumentNullException(nameof(rules));

        public void TakeSpk(SpkOffer offer, double hour)
        {
            if (Position.Distance(Haul.Gate) > Haul.DeskReach) throw new InvalidOperationException("Datangi pos timbang pabrik.");
            if (!Yard.bandar) throw new InvalidOperationException("SPK hanya untuk bandar Kasta 2 ke atas.");
            if (Spk.active) throw new InvalidOperationException("Selesaikan SPK yang berjalan dahulu.");
            if (offer.Id / 10 != SpkRules.Day(hour) || Spk.taken.Contains(offer.Id))
                throw new InvalidOperationException("SPK ini sudah tidak tersedia.");
            if (Yard.reputation < offer.MinReputation)
                throw new InvalidOperationException("Reputasi minimal " + offer.MinReputation + ".");
            if (!HasLoader || Yard.Machine(true, hour).condition <= SpkTerms.LoaderCondition)
                throw new InvalidOperationException("Loader harus berkondisi di atas " + SpkTerms.LoaderCondition + "%.");
            double bond = offer.Value * SpkTerms.Bond;
            if (Cash < bond) throw new InvalidOperationException("Kas belum cukup untuk jaminan 5%.");
            Pay(bond);
            Spk = new SpkSave
            {
                active = true, id = offer.Id, grade = offer.Grade, kg = offer.Kg, perKg = offer.PerKg, bond = bond,
                deadline = hour + offer.Hours,
                taken = Spk.taken.Where(id => id / 10 == offer.Id / 10).Append(offer.Id).ToArray()
            };
        }

        /// <summary>How much of this load the running order takes at its own price: the right grade, up to what is still owed.</summary>
        public double SpkKg(TruckLoad load) =>
            Spk.active && load.grade == Spk.grade ? Math.Min(load.billableKg, Spk.Remaining) : 0;

        private void DeliverSpk(double kg, double hour)
        {
            if (kg <= 0) return;
            Spk.delivered += kg;
            if (Spk.Remaining > 1e-6) return;
            double late = Math.Ceiling(Math.Max(0, hour - Spk.deadline));
            double penalty = Math.Min(Spk.bond, Spk.Value * SpkTerms.LatePerHour * late);
            CreditCash(Spk.bond - penalty);
            if (penalty > 0) RecordOperatingProfit(-penalty);
            else Yard.reputation = (int)CrewSave.Meter(Yard.reputation + SpkTerms.OnTimeReputation);
            Spk.note = penalty > 0 ? "SPK selesai terlambat " + late + " jam. Denda dipotong dari jaminan."
                : "SPK selesai tepat waktu. Jaminan kembali, reputasi +" + SpkTerms.OnTimeReputation + ".";
            Spk.active = false;
        }

        /// <summary>An order still open when the late penalty has eaten the bond is lost with it.</summary>
        public void AdvanceSpk(double hour)
        {
            if (!Spk.active || hour < Spk.deadline + SpkTerms.Grace) return;
            RecordOperatingProfit(-Spk.bond);
            Yard.reputation = (int)CrewSave.Meter(Yard.reputation - SpkTerms.FailReputation);
            Spk.note = "SPK gagal. Jaminan hangus, reputasi -" + SpkTerms.FailReputation + ".";
            Spk.active = false;
        }
    }
}
