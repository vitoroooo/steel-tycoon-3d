using System.Linq;

namespace Steel.Services
{
    public partial interface IGameService
    {
        /// <summary>The legacies this career started with (PRD 11.3).</summary>
        string[] Legacies { get; }
        /// <summary>True while the career may still take a veteran's legacies: before its first sale, once.</summary>
        bool LegacyOpen { get; }
        /// <summary>Starts this career with the legacies picked for the season; returns what to tell the player.</summary>
        string StartWithLegacies(string[] picks);
    }

    public sealed partial class LocalGameService
    {
        public string[] Legacies => session.Legacies.ToArray();
        public bool LegacyOpen => session.LegacyOpen;

        public string StartWithLegacies(string[] picks)
        {
            session.StartWithLegacies(picks, FleetCatalog.Triseda);
            return "Bekal dipakai di karier ini: " + string.Join(", ", session.Legacies.Select(LegacyText.Effect)) + ".";
        }
    }
}
