using System;
using System.Linq;

namespace Steel.Core.Risk
{
    /// <summary>What the yard's trouble looks like right now: heat, a gang at the gate, the guards posted. Saved as is.</summary>
    [Serializable] public sealed class RiskSave
    {
        public double heat;
        /// <summary>The last game day the heat cooled, a gang was rolled for, and the contraband offer was answered.</summary>
        public long cooledDay, rolledDay = -1, offerDay = -1;
        /// <summary>A gang is at the gate: what they ask, the game hour they stop waiting, how many came, whether talks were tried.</summary>
        public bool demand, haggled;
        public double demandDue, demandUntil;
        public int thugs;
        /// <summary>No gang before this game hour; the yard is sealed until this one after a raid at Kasta 3.</summary>
        public double quietUntil, seizedUntil;
        /// <summary>Sales this game month and the month before; a gang asks a share of the larger.</summary>
        public double turnover, lastMonth;
        public long month;
        public int[] guards = new int[RiskRules.Cells];
        /// <summary>Posts whose guards were knocked out in the last fight; they are back on their feet at this game hour.</summary>
        public bool[] fainted = new bool[RiskRules.Cells];
        public double faintUntil;
        /// <summary>When nobody answers in time: hold the gate instead of paying.</summary>
        public bool defend;
        /// <summary>A paid protector halves raids until this hour; once exposed, the next penalty doubles.</summary>
        public double protectorUntil;
        public bool exposed;
        public int sequence;
        public string note;

        /// <summary>The grid as it can fight right now: without the guards still out cold.</summary>
        public int[] OnDuty(double hour) => guards.Select((kind, cell) => hour < faintUntil && fainted[cell] ? 0 : kind).ToArray();
        public bool Protected(double hour) => hour < protectorUntil;

        internal static RiskSave Restore(RiskSave saved)
        {
            if (saved == null || saved.guards == null || saved.guards.Length == 0) return new RiskSave();
            if (saved.fainted == null || saved.fainted.Length == 0) saved.fainted = new bool[RiskRules.Cells];
            if (saved.guards.Length != RiskRules.Cells || saved.fainted.Length != RiskRules.Cells ||
                saved.guards.Where((kind, cell) => kind < 0 || kind > RiskRules.Kinds || kind > 0 && cell >= RiskRules.Posts).Any() ||
                !(saved.heat >= 0) || !(saved.demandDue >= 0) || !(saved.turnover >= 0) || !(saved.lastMonth >= 0) ||
                saved.thugs < 0 || saved.sequence < 0 || saved.demand && saved.thugs == 0)
                throw new FormatException("Data keamanan rusak.");
            return saved;
        }

        /// <summary>The next number in this career's own sequence, so a reload cannot roll again.</summary>
        internal bool Roll(double chance)
        {
            if (chance <= 0) return false;
            sequence = sequence == int.MaxValue ? 1 : sequence + 1;
            uint hash = unchecked((uint)sequence * 747796405u + 2891336453u);
            hash = unchecked(((hash >> (int)((hash >> 28) + 4)) ^ hash) * 277803737u);
            return ((hash >> 22) ^ hash) / 4294967296d < chance;
        }
    }
}
