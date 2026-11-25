using System;
using Steel.Core.Events;

namespace Steel.Core.State.Slice
{
    /// <summary>"Setoran berkotoran" (PRD appendix E, Kasta 2): a cheating collector puts a load full of dirt on the scale.</summary>
    public sealed partial class SliceSession
    {
        /// <summary>Called for each collector's load before it is weighed. True when the scale is taken by a disputed load, so the
        /// usual receipt must not happen this hour. A load nobody answers is taken with the cut when the collector's wait is over.</summary>
        public bool HoldDeposit(int grade, double kg, double floorRate, double capacity, long hour)
        {
            EventSave e = Events;
            if (e.DirtyWaiting)
            {
                if (hour >= e.dirtUntil) WeighDirty(capacity, hour, "Setoran berkotoran diterima otomatis dengan potongan.");
                return true;
            }
            if (!Yard.bandar || !HasScale || hour >= Yard.leaseUntil || Yard.bids[grade] < floorRate || hour < e.honestUntil ||
                !Dice.Roll(ref e.sequence, EventTerms.Get("dirtyChance"))) return false;
            double low = EventTerms.Get("dirtMin"), high = EventTerms.Get("dirtMax");
            e.dirtGrade = grade; e.dirtKg = kg; e.dirt = low + (high - low) * Dice.Next(ref e.sequence);
            e.dirtUntil = hour + EventTerms.Get("dirtyAnswerHours");
            e.note = "Setoran berkotoran di timbangan: " + kg.ToString("N0") + " kg, kotoran " + (e.dirt * 100).ToString("0") + "%.";
            return true;
        }

        /// <summary>Takes the load with the cut: only the metal is weighed in and paid for at the board price.</summary>
        public void AcceptDirtyDeposit(double capacity, long hour)
        {
            RequireDirty();
            if (!WeighDirty(capacity, hour, "Setoran diterima dengan potongan kotoran."))
                throw new InvalidOperationException("Kas, kapasitas lapak, atau timbangan tidak cukup untuk menerima setoran ini.");
        }

        /// <summary>Turns the load away: the cheat's name suffers among collectors, and loads arrive clean for a while.</summary>
        public void RejectDirtyDeposit(long hour)
        {
            RequireDirty();
            Events.dirtKg = 0;
            Events.honestUntil = hour + EventTerms.Get("honestHours");
            Events.note = "Setoran berkotoran ditolak. Penyetor lain jadi lebih jujur beberapa hari.";
        }

        private void RequireDirty()
        {
            if (!Events.DirtyWaiting) throw new InvalidOperationException("Tidak ada setoran berkotoran di timbangan.");
        }

        private bool WeighDirty(double capacity, long hour, string note)
        {
            EventSave e = Events;
            double before = Yard.stock[e.dirtGrade];
            Receive(e.dirtGrade, 0, e.dirtKg * (1 - e.dirt), capacity, hour);
            bool taken = Yard.stock[e.dirtGrade] > before;
            // a load answered by nobody that the yard cannot pay for or hold simply leaves
            if (!taken && hour < e.dirtUntil) return false;
            e.dirtKg = 0; e.note = taken ? note : "Penyetor berkotoran pergi: lapak tidak bisa menerima.";
            return taken;
        }
    }
}
