using System.Linq;
using UnityEngine;
using Steel.Services.Map;

namespace Steel.Editor.Vehicles.Trucks
{
    /// <summary>Stands the heavy fleet on the truck dealer's forecourt, nose to the street. Each truck's bay is its spawn in Config.</summary>
    internal static class TruckPool
    {
        internal static void Create(Transform player)
        {
            MapSite dealer = MapPlan.Load().sites.First(site => site.kind == MapSite.TruckDealer);
            foreach (TruckSpec spec in TruckSpec.All) TruckBuilder.Create(spec, player, dealer.yaw);
        }
    }
}
