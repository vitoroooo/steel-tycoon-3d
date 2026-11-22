using System.Globalization;
using Steel.Core.Trade.Street;

namespace Steel.Services
{
    /// <summary>What sellers are called and what they say when bargained with.</summary>
    internal static class StreetText
    {
        private static readonly string[] Titles = { "Rumah warga", "Bengkel", "Proyek bongkaran", "Lapak pasar" };

        internal static string Title(SellerKind kind) => Titles[(int)kind];

        internal static string Rupiah(double amount) => "Rp" + amount.ToString("N0", CultureInfo.GetCultureInfo("id-ID"));

        internal static string Reply(BargainReply reply, double perKg)
        {
            string price = Rupiah(perKg) + " sekilo";
            switch (reply)
            {
                case BargainReply.Deal: return "\"Ya sudah, " + price + ". Jadi.\"";
                case BargainReply.Counter: return "\"Belum dapat segitu. " + price + " deh.\"";
                case BargainReply.Offended: return "\"Itu bukan nawar namanya. Tetap " + price + ".\"";
                default: return "\"Harga pas, " + price + ". Mau, ambil.\"";
            }
        }
    }
}
