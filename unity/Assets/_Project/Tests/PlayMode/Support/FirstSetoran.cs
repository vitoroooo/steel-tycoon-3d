using System.Collections;
using UnityEngine;
using Steel.Presentation.UI;
using Steel.Presentation.Vehicles;
using Steel.Services;

namespace Steel.PlayTests.Support
{
    /// <summary>Earns the motor bak the way a new player does: opening cash, one sale, Pak Darto's grant.</summary>
    internal static class FirstSetoran
    {
        /// <summary>Ends with the player riding the triseda at the dealer. No cash is injected.</summary>
        internal static IEnumerator RideTriseda(GameHost host, CityHud hud)
        {
            var pile = host.Layout.Piles[0].Position;
            OwnedFleet.PlacePlayer(host, pile.X, pile.Z);
            yield return null;
            hud.Act(); SortGesture.Drop(hud, "HMS 1"); hud.Act();
            OwnedFleet.PlacePlayer(host, host.Layout.Home.X, host.Layout.Home.Z);
            hud.Act(); SortGesture.Drop(hud, "HMS 1");
            OwnedFleet.PlacePlayer(host, host.Layout.Factory.X, host.Layout.Factory.Z);
            hud.Act(); hud.Confirm();
            VehicleView bike = host.Game.VehicleOf(FleetCatalog.Triseda);
            OwnedFleet.PlacePlayer(host, bike.X, bike.Z);
            hud.Act();   // Pak Darto's grant
            hud.Act();   // buy
            hud.Act();   // get on
            yield return null;
        }
    }
}
