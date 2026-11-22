using System;
using Steel.Presentation.World.Mobile;
using UnityEngine;

namespace Steel.Presentation.UI.Boot
{
    /// <summary>The graphics profile as the start menu sees it: a name and a way to step to the next one.</summary>
    public sealed class GraphicsOption
    {
        private readonly Func<string> label;
        private readonly Action cycle;
        public string Label => label();
        public void Cycle() => cycle();

        public GraphicsOption(Func<string> label, Action cycle)
        {
            this.label = label ?? throw new ArgumentNullException(nameof(label));
            this.cycle = cycle ?? throw new ArgumentNullException(nameof(cycle));
        }

        /// <summary>Phones get the saved profile, as on the in-game HUD; elsewhere the menu has no such button.</summary>
        public static GraphicsOption ForDevice()
        {
            if (!Application.isMobilePlatform) return null;
            MobileQuality.Restore();
            return new GraphicsOption(() => MobileQuality.Label, MobileQuality.Cycle);
        }
    }
}
