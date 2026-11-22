using System;
using Steel.Services;

namespace Steel.Presentation.UI
{
    /// <summary>Tells the player where the next rupiah comes from, also once the street has been picked clean.</summary>
    internal static class IncomeHint
    {
        private static string Minutes(double m) => Math.Max(1, Math.Ceiling(m)) + " menit";

        internal static string Text(IncomeView income, GameView view, CityLayout map)
        {
            string home = "Lapak " + DartoDirections.To(view, map.Home.X, map.Home.Z);
            if (income.DepotKg > 0)
                return home + " · Kiriman " + income.DepotKg + " kg sudah tiba. Buka armada, ketuk AMBIL KIRIMAN.";
            if (income.StreetSites > 0)
                return "Gang " + DartoDirections.To(view, income.SiteX, income.SiteZ) + " · " +
                    (income.SiteKg > view.CapacityKg ? "Tumpukan " + income.SiteKg + " kg, bawa gerobak. "
                        : "Periksa tumpukan, lalu pungut. ") + "Sisa " + income.StreetSites + " tumpukan.";
            string next = "Warga menaruh tumpukan baru " + Minutes(income.RestockMinutes) + " lagi.";
            if (income.ArrivalMinutes > 0)
                return "Gang kosong. Kiriman rute tiba di lapak " + Minutes(income.ArrivalMinutes) + " lagi. " + next;
            if (income.RouteKg > 0)
                return "Gang kosong. " + home + " · Buka armada, pesan rute gerobak gratis (" + income.RouteKg +
                    " kg). " + next;
            return "Gang kosong. " + next;
        }

        /// <summary>Pak Darto's answer to "the street is bare, where does money come from now?".</summary>
        internal static string Letter(IncomeView income) =>
            "Pak Darto  ·  Gang sudah bersih?\nTenang. Warga menaruh rongsok baru tiap beberapa menit" +
            (income.RouteKg > 0 ? ", dan dari lapak gerobakmu bisa memungut " + income.RouteKg +
                " kg gratis: BUKA ARMADA → PESAN RUTE.  ›" : ". Pantau petunjuk di bawah layar.  ›");
    }
}
