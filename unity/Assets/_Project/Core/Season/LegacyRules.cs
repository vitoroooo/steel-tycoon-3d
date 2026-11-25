using System;
using Steel.Core.State;

namespace Steel.Core.Season
{
    /// <summary>The five legacies a veteran may bring to a new city (PRD 11.3), with their numbers from Legacy.json.</summary>
    public sealed class LegacyRules
    {
        public const string Triseda = "triseda", Reputation = "reputasi", Sorting = "sortir", Queue = "antrean", Company = "pt";
        public static readonly string[] All = { Triseda, Reputation, Sorting, Queue, Company };

        private readonly ConfigValues values;
        /// <summary>Real seconds in one game hour, to turn the queue legacy's game hours into offline time.</summary>
        public double HourSeconds { get; }

        public LegacyRules(string json, double hourSeconds)
        {
            values = new ConfigValues(json);
            if (!(hourSeconds > 0)) throw new ArgumentOutOfRangeException(nameof(hourSeconds));
            HourSeconds = hourSeconds;
        }

        public int Picks => (int)values.Positive("picks");
        public int StartReputation => (int)values.Positive("reputation");
        /// <summary>Share of the usual time a collecting-and-sorting round takes with the sorting legacy.</summary>
        public double RouteFactor => values.Fraction("routeFactor");
        public double QueueSeconds => values.Positive("queueHours") * HourSeconds;

        /// <summary>The picks as they will be kept: known ids only, no repeats, never more than the allowance.</summary>
        public string[] Checked(string[] picks)
        {
            if (picks == null || picks.Length == 0) throw new InvalidOperationException("Belum ada bekal yang dipilih.");
            if (picks.Length > Picks) throw new InvalidOperationException("Paling banyak " + Picks + " bekal.");
            for (int i = 0; i < picks.Length; i++)
                if (Array.IndexOf(All, picks[i]) < 0 || Array.IndexOf(picks, picks[i]) != i)
                    throw new InvalidOperationException("Bekal tidak dikenal atau dipilih dua kali.");
            return (string[])picks.Clone();
        }
    }
}
