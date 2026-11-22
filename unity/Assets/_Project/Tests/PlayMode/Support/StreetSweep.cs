using Steel.Services;

namespace Steel.PlayTests.Support
{
    /// <summary>Works the street with the gerobak the way a new player does, on the opening cash only.</summary>
    internal static class StreetSweep
    {
        /// <summary>Picks and sells street sites until only <paramref name="keep"/> still hold scrap.</summary>
        internal static void Until(GameHost host, int keep)
        {
            var g = host.Game;
            if (!g.Cart.Held) { OwnedFleet.PlacePlayer(host, g.Cart.X, g.Cart.Z); g.GrabCart(); }
            for (var next = g.Income; next.StreetSites > keep; next = g.Income)
            {
                if (next.SiteKg > g.View.CapacityKg - g.View.CarriedKg) { Sell(host); continue; }
                OwnedFleet.PlacePlayer(host, next.SiteX, next.SiteZ);
                string id = g.Nearby.Id; g.Inspect(id); g.PickUp(id);
            }
            Sell(host);
        }

        /// <summary>Sorts at the lapak, sells steel to the factory and ships the rest to Toko Bangunan.</summary>
        internal static void Sell(GameHost host)
        {
            var g = host.Game; var map = host.Layout;
            if (g.View.CarriedKg == 0) return;
            OwnedFleet.PlacePlayer(host, map.Home.X, map.Home.Z); g.SortAtHome();
            if (g.HasSteel)
            {
                OwnedFleet.PlacePlayer(host, map.Factory.X, map.Factory.Z); g.PreviewSale(); g.ConfirmSale();
            }
            if (g.View.CarriedKg == 0) return;
            OwnedFleet.PlacePlayer(host, map.Home.X, map.Home.Z); g.InspectBid(g.BuyerCount - 1); g.SellLocal();
        }
    }
}
