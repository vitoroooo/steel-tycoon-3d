using System;
using System.Collections.Generic;
using Steel.Editor.Plan.Land;
using Steel.Editor.Plan.Places.Builders;
using Steel.Editor.Plan.Town;
using Steel.Services.Map;

namespace Steel.Editor.Plan.Places
{
    /// <summary>Builds the lots the plan fixes by hand, before the places around them are packed. A new kind of site is one line here.</summary>
    internal static class Sites
    {
        private const float Gap = .3f;
        private static readonly Dictionary<string, Action<TownKit, Lot>> Builders = new Dictionary<string, Action<TownKit, Lot>>
        {
            { MapSite.TruckDealer, Dealers.Trucks }, { MapSite.PlayerLapak, Plots.Lapak }, { MapSite.PlayerFactory, Plots.Factory }
        };

        internal static int Create(LandGrid grid, TownKit kit)
        {
            foreach (MapSite site in grid.Plan.sites)
            {
                var lot = new Lot(site.Centre, site.Front, site.width, site.depth, site.name.GetHashCode());
                if (!grid.Fits(grid.Owner(site.Centre), lot, Gap))
                    throw new InvalidOperationException(site.name + " does not fit on one place: move it in docs/design/peta/sukabaja.json");
                grid.Take(lot, Gap);
                Builders[site.kind](kit, lot);
            }
            return grid.Plan.sites.Length;
        }
    }
}
