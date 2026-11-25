using System;

namespace Steel.Tests.Support
{
    internal sealed partial class Scavenger
    {
        /// <summary>Picks and sells until no street site holds scrap.</summary>
        internal void ClearTheStreet()
        {
            for (int step = 0; Game.Income.StreetSites > 0; step++)
            {
                if (step > 80) throw new InvalidOperationException("The street never ran out.");
                if (!Gather()) SellAll();
            }
            SellAll();
        }

        /// <summary>Does the one thing the HUD hint asks for; <paramref name="rounds"/> false ignores the cart round.</summary>
        internal void FollowHint(bool rounds = true)
        {
            var income = Game.Income;
            if (income.DepotKg > 0) { Home(); TakeCart(); Game.CollectRoute(); SellAll(); }
            else if (Game.View.CarriedKg > 0) SellAll();
            else if (Gather()) return;
            else if (rounds && income.ArrivalMinutes == 0 && income.RouteKg > 0)
            { Home(); Game.BookRoute(Game.Route("cart", 0, false)); }
            else Wait(income.ArrivalMinutes > 0 ? Math.Min(income.ArrivalMinutes, income.RestockMinutes)
                : income.RestockMinutes);
        }
    }
}
