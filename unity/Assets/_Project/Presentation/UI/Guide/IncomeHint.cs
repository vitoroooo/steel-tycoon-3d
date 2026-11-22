using System;
using Steel.Services;

namespace Steel.Presentation.UI
{
    /// <summary>Tells the player where the next rupiah comes from, also once the street has been picked clean.</summary>
    public static class IncomeHint
    {
        private static string Minutes(double m) => Math.Max(1, Math.Ceiling(m)) + " menit";

        /// <summary><paramref name="brief"/> keeps the hint on one line when a vehicle line is shown below it.</summary>
        public static string Text(IncomeView income, GameView view, CityLayout map, bool brief = false)
        {
            string home = "Lapak " + Compass.To(view, map.Home.X, map.Home.Z);
            if (income.DepotKg > 0)
                return home + " · Kiriman " + income.DepotKg + " kg sudah datang. BUKA ARMADA, AMBIL KIRIMAN.";
            if (income.StreetSites > 0) return Street(income, view);
            string next = "Tumpukan baru muncul " + Minutes(income.RestockMinutes) + " lagi.";
            if (income.ArrivalMinutes > 0)
                return "Gang kosong. Kiriman gerobak sampai di lapak " + Minutes(income.ArrivalMinutes) + " lagi." +
                    (brief ? "" : "\n" + next);
            if (income.RouteKg > 0)
                return "Gang kosong. " + home + " · BUKA ARMADA, pesan rute gratis " + income.RouteKg + " kg (" +
                    Minutes(income.RouteMinutes) + ")." + (brief ? "" : "\n" + next);
            return "Gang kosong. " + next;
        }

        private static string Street(IncomeView income, GameView view)
        {
            string site = "Gang " + Compass.To(view, income.SiteX, income.SiteZ) + " · ";
            double room = view.CapacityKg - view.CarriedKg;
            if (income.SiteKg <= room)
                return site + "Periksa dulu, baru pungut. Sisa " + income.StreetSites + " tumpukan.";
            return site + "Tumpukan " + income.SiteKg + " kg, cuma muat " + room + " kg. " +
                (income.CartAwayMinutes > 0 ? "Ambil dulu, gerobak pulang " + Minutes(income.CartAwayMinutes) + " lagi."
                    : "Ambil semuatnya, sisanya aman.");
        }

        /// <summary>Pak Darto's answer to "the street is bare, where does money come from now?".</summary>
        public static string Letter(IncomeView income) =>
            "Pak Darto  ·  Gang kosong?\nTiap " + Minutes(income.RestockEveryMinutes) + " warga buang rongsok lagi." +
            (income.RouteKg > 0 ? " Daripada bengong, ke lapak: BUKA ARMADA, PESAN RUTE. Gerobak pulang " +
                Minutes(income.RouteMinutes) + " lagi bawa " + income.RouteKg + " kg, gratis.  ›" : " Lihat petunjuk di bawah layar.  ›");
    }
}
