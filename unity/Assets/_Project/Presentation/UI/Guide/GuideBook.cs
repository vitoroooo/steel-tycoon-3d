using System;
using System.Globalization;
using Steel.Services;

namespace Steel.Presentation.UI
{
    /// <summary>
    /// Pak Darto's notes for a beginner, kept in Buku usaha. The player opens them when they want to;
    /// nothing here pops up. Figures are read from the running game so the notes never drift from the rules.
    /// </summary>
    public static class GuideBook
    {
        public static readonly string[] Topics =
            { "Duit dari mana?", "Tas & gerobak", "Sortir & jual", "Motor & rute", "Kalau hujan" };

        /// <summary>Idle prompt line that says where the notes are, until the player has opened them.</summary>
        public const string Pointer = "Bingung? Buku usaha › PANDUAN";

        public static string Idle(bool guideRead) => guideRead
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
                case 0: return "Gang habis? Tenang, tiap " + Minutes(income.RestockEveryMinutes) + " warga buang " +
                    "rongsok lagi, satu tumpukan. Daripada bengong, kirim gerobak keliling: di lapak ketuk BUKA ARMADA, " +
                    "lalu PESAN RUTE. " + Minutes(income.RouteMinutes) + " lagi dia pulang bawa " + income.RouteKg +
                    " kg, tanpa keluar uang sepeser pun.";
                case 1: return "Tanpa gerobak, kamu cuma kuat bawa sekantong. Dorong gerobak, muat " +
                    game.Cart.CapacityKg + " kg. Tumpukannya terlalu berat? Ambil semuatnya dulu, sisanya tidak ke " +
                    "mana-mana. Mau pegang atau lepas gerobak, berdiri di sebelahnya lalu ketuk tombol aksi.";
                case 2: return "Rongsok yang belum disortir tidak laku. Di lapak ketuk SORTIR, lalu seret tiap sampel " +
                    "ke bak yang sesuai. Besi bawa ke Pabrik AI, tapi cek PAPAN HARGA dulu, siapa tahu bandar berani " +
                    "lebih. Barang layak pakai larinya ke Toko Bangunan.";
                case 3: return Fleet(game);
                default: return "Hujan bikin HMS 1 dan barang layak pakai karatan, harganya ikut turun. Intip " +
                    "prakiraan di halaman Cuaca. Kalau bakal hujan, bawa stok pulang dan pilih Titip di teras. " +
                    "Di bawah atap, besimu aman.";
            }
        }

        private static string Fleet(IGameService game)
        {
            var bike = game.VehicleOf(FleetCatalog.Triseda);
            var round = game.Route(FleetCatalog.Triseda, 0, false);
            return "Begitu setoran pertamamu beres, mampir ke dealer. Ada hadiah dari saya buat modal motor bak " +
                "roda tiga (" + Rp(bike.Price) + "). Motor itu sekali jalan " + Minutes(round.Minutes) +
                ", pulang bawa " + round.Kg + " kg, gratis juga. Rute beli lain cerita, itu pakai kas. Hitung dulu.";
        }
    }
}
