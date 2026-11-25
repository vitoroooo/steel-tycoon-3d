using System;
using Steel.Core.Season;

namespace Steel.Core.State.Slice
{
    /// <summary>Bekal warisan (PRD 11.3): a small head start a veteran gives a career that has not sold anything yet.</summary>
    public sealed partial class SliceSession
    {
        private LegacyRules legacyRules;
        /// <summary>The legacies this career started with; empty for a newcomer.</summary>
        public string[] Legacies { get; private set; } = new string[0];

        public void OpenLegacies(LegacyRules rules) =>
            legacyRules = rules ?? throw new ArgumentNullException(nameof(rules));

        public bool HasLegacy(string id) => Array.IndexOf(Legacies, id) >= 0;
        /// <summary>Legacies only speed up the start, so they are taken before the first sale and once.</summary>
        public bool LegacyOpen => legacyRules != null && !FirstSaleMade && Legacies.Length == 0;

        public void StartWithLegacies(string[] picks, string starterVehicle)
        {
            if (!LegacyOpen) throw new InvalidOperationException("Bekal hanya untuk karier baru yang belum pernah menjual.");
            Legacies = legacyRules.Checked(picks);
            var starter = Fleet.Find(starterVehicle);
            if (HasLegacy(LegacyRules.Triseda) && !starter.Owned) starter.Buy();
            if (HasLegacy(LegacyRules.Reputation)) Yard.reputation = Math.Max(Yard.reputation, legacyRules.StartReputation);
            if (HasLegacy(LegacyRules.Company) && !Yard.applied) { Yard.applied = true; Yard.permitHour = 0; }
        }

        /// <summary>How long a collecting round takes for this career.</summary>
        public double RouteSeconds(double standard) =>
            HasLegacy(LegacyRules.Sorting) ? standard * legacyRules.RouteFactor : standard;

        private double LegacyQueueSeconds => HasLegacy(LegacyRules.Queue) ? legacyRules.QueueSeconds : 0;

        private void RestoreLegacies(string[] saved)
        {
            Legacies = saved ?? new string[0];
            if (Legacies.Length > 0)
                Legacies = (legacyRules ?? throw new FormatException("Legacy rules")).Checked(Legacies);
        }
    }
}
