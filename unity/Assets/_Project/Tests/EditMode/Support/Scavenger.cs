using System;
using Steel.Presentation.UI;
using Steel.Services;

namespace Steel.Tests.Support
{
    /// <summary>
    /// Plays the opening career the way a new player does: unmodified config, starting cash only, on foot
    /// behind the gerobak with the clock running while walking. It never claims the grant or buys anything.
    /// </summary>
    internal sealed partial class Scavenger
    {
        internal readonly LocalGameService Game;
        internal readonly CityLayout Map;
        /// <summary>The longest stretch, in real minutes, the player had nothing to do but wait.</summary>
        internal double LongestWait { get; private set; }

        internal Scavenger()
        {
            var source = new ResourceConfigSource();
            Game = new LocalGameService(source); Map = new CityLayout(source);
            TakeCart();
        }

        internal void Walk(float x, float z)
        {
            var v = Game.View;
            double metres = Math.Sqrt(Math.Pow(x - v.PlayerX, 2) + Math.Pow(z - v.PlayerZ, 2));
            Game.Advance(metres / Game.Cart.PushSpeed);
            Game.SetPosition(x, z);
        }

        internal void Home() => Walk(Map.Home.X, Map.Home.Z);

        /// <summary>Takes the push bar unless the gerobak is away on its round.</summary>
        internal void TakeCart()
        {
            if (Game.Cart.Held || Game.Income.CartAwayMinutes > 0) return;
            Walk(Game.Cart.X, Game.Cart.Z); Game.GrabCart();
        }

        internal void Wait(double realMinutes)
        {
            LongestWait = Math.Max(LongestWait, realMinutes);
            Game.Advance(realMinutes * 60 + 1);
        }

        /// <summary>The HUD hint line, and the earning line it falls back to when no one-off errand is pending.</summary>
        internal string Hint => DartoDirections.Hint(Game, Game.View, Map);
        internal string Earning => IncomeHint.Text(Game.Income, Game.View, Map);

        /// <summary>Walks to the site the HUD points at and loads what fits; false when full or no site is left.</summary>
        internal bool Gather()
        {
            TakeCart();
            var income = Game.Income;
            if (income.StreetSites == 0 || Game.View.CarriedKg >= Game.View.CapacityKg) return false;
            Walk(income.SiteX, income.SiteZ);
            string id = Game.Nearby.Id; Game.Inspect(id); Game.PickUp(id);
            return true;
        }

        /// <summary>Sorts at the lapak, sells steel at the factory and ships anything reusable to Toko Bangunan.</summary>
        internal void SellAll()
        {
            if (Game.View.CarriedKg == 0) return;
            Home(); Game.SortAtHome();
            if (Game.HasSteel)
            {
                Walk(Map.Factory.X, Map.Factory.Z); Game.PreviewSale(); Game.ConfirmSale();
            }
            if (Game.View.CarriedKg == 0) return;
            Home(); Game.InspectBid(Game.BuyerCount - 1); Game.SellLocal();
        }
    }
}
