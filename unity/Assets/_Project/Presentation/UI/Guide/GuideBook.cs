using System;
using System.Globalization;
using Steel.Services;

namespace Steel.Presentation.UI
{
    /// <summary>
    /// The beginner's reference in Buku usaha. The player opens it when they want to; nothing here pops up.
    /// Figures are read from the running game so the text never drifts from the rules.
    /// </summary>
    public static class GuideBook
    {
        public static readonly string[] Topics =
            { "Dari mana uang?", "Muatan", "Sortir & jual", "Armada", "Cuaca" };

        /// <summary>Idle prompt line that tells the player where this reference lives, until they have opened it.</summary>
        public const string Pointer = "Panduan: Buku usaha › PANDUAN";

        public static string Idle(bool guideSeen) => guideSeen
            ? "Geser kiri untuk bergerak · Usap kanan untuk melihat"
            : "Geser kiri: bergerak · Usap kanan: melihat\n" + Pointer;

        private static string Minutes(double m) => Math.Max(1, Math.Ceiling(m)) + " menit";
        private static string Rp(double n) => "Rp" + n.ToString("N0", CultureInfo.GetCultureInfo("id-ID"));

        public static string Heading(int topic) => (topic + 1) + "/" + Topics.Length + "  ·  " + Topics[topic];

        public static string Page(int topic, IGameService game)
        {
            var income = game.Income;
            switch (topic)
            {
                case 0: return "Pungut rongsok di gang, sortir di lapak, lalu jual. Tumpukan yang habis diisi warga lagi, " +
                    "satu tumpukan tiap " + Minutes(income.RestockEveryMinutes) + ". Di lapak, BUKA ARMADA lalu PESAN RUTE: " +
                    "gerobak pulang membawa " + income.RouteKg + " kg gratis dalam " + Minutes(income.RouteMinutes) + ".";
                case 1: return "Tanpa gerobak Anda hanya membawa tas tangan; gerobak memuat " + game.Cart.CapacityKg +
                    " kg. Tumpukan yang lebih berat dipungut sebagian, sisanya menunggu di gang. " +
                    "Berdiri di dekat gerobak lalu ketuk tombol aksi untuk mendorong atau melepasnya.";
                case 2: return "Di lapak ketuk SORTIR, lalu seret sampel ke bak yang sesuai. Besi dijual ke " +
                    "Pabrik AI atau ke bandar lewat PAPAN HARGA; barang layak pakai ke Toko Bangunan. " +
                    "Bandingkan harga dan ongkos jalan sebelum menjual.";
                case 3: return Fleet(game);
                default: return "Hujan membuat HMS 1 dan barang layak pakai berkarat dan harganya turun. " +
                    "Lihat prakiraan di Buku usaha › Cuaca. Bawa stok ke lapak, lalu di halaman Cuaca pilih Titip di teras.";
            }
        }

        private static string Fleet(IGameService game)
        {
            var bike = game.VehicleOf(FleetCatalog.Triseda);
            var round = game.Route(FleetCatalog.Triseda, 0, false);
            return "Setoran pertama membuka hadiah Pak Darto di dealer. Motor bak roda tiga (" + Rp(bike.Price) +
                ") pulang membawa " + round.Kg + " kg tiap " + Minutes(round.Minutes) + " rute pungut gratis. " +
                "Rute beli memakai kas, jadi pesan hanya bila kas cukup.";
        }
    }
}
