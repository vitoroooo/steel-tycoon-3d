using System;

namespace Steel.Core.Trade.Spk
{
    /// <summary>The work order the player is filling, if any, and the ones already taken from today's board. Saved as is.</summary>
    [Serializable] public sealed class SpkSave
    {
        public bool active;
        public long id;
        public int grade;
        /// <summary>Ordered and delivered so far; the fixed price; the deposit the mill holds; the game hour it is due.</summary>
        public double kg, delivered, perKg, bond, deadline;
        public long[] taken = new long[0];
        /// <summary>How the last order ended, for the board to show.</summary>
        public string note;

        public double Value => kg * perKg;
        public double Remaining => active ? Math.Max(0, kg - delivered) : 0;

        internal static SpkSave Restore(SpkSave saved)
        {
            if (saved == null) return new SpkSave();
            if (saved.taken == null || saved.grade < 0 || saved.grade > 3 || !(saved.kg >= 0) || !(saved.delivered >= 0) ||
                !(saved.perKg >= 0) || !(saved.bond >= 0) || saved.active && (saved.kg <= 0 || saved.delivered >= saved.kg))
                throw new FormatException("SPK rusak.");
            return saved;
        }
    }
}
