using System;
using System.Globalization;

namespace Steel.Services
{
    /// <summary>Every sentence of the start screens, in one place and free of Unity.</summary>
    public static class MenuText
    {
        public const string Skip = "Ketuk untuk lanjut", NewCareer = "KARIER BARU";
        public const string ConfirmYes = "YA, MULAI BARU", ConfirmNo = "BATAL";
        /// <summary>Whole rupiah with dots between thousands, the same on every phone language.</summary>
        public static string Money(double value)
        {
            var digits = Math.Floor(Math.Abs(value)).ToString("F0", CultureInfo.InvariantCulture);
            for (int i = digits.Length - 3; i > 0; i -= 3) digits = digits.Insert(i, ".");
            return (value < 0 ? "-Rp " : "Rp ") + digits;
        }

        public static string Play(CareerState state) =>
            state == CareerState.Saved ? "LANJUTKAN" : state == CareerState.Empty ? "MULAI KARIER" : "MAIN TANPA SIMPAN";

        public static string CardTitle(CareerState state) =>
            state == CareerState.Saved ? "KARIER TERSIMPAN" :
            state == CareerState.Empty ? "KARIER PERTAMA" : "SIMPANAN TIDAK TERBACA";

        public static string CardBody(CareerSummary career, long nowUtc)
        {
            if (career.State == CareerState.Empty)
                return "Mulai usaha rongsok dari nol di Sukabaja.\nProgres tersimpan otomatis di HP ini.";
            if (career.State == CareerState.Unreadable)
                return "Progres lama tidak bisa dibuka.\nMain tanpa simpan tidak akan menimpanya.";
            return "Hari ke-" + career.Day + " · Kasta " + career.Caste + "\nKas " + Money(career.Cash) +
                "\nTerakhir main " + Ago(nowUtc - career.SavedUtc);
        }

        public static string Confirm(CareerSummary career) => career.State == CareerState.Saved
            ? "Mulai karier baru?\nKarier Hari ke-" + career.Day + " dengan kas " + Money(career.Cash) +
              " akan diganti dan tidak bisa dilanjutkan lagi."
            : "Mulai karier baru?\nSimpanan lama yang tidak terbaca akan diganti.";

        public static string Ago(long seconds)
        {
            if (seconds < 60) return "baru saja";
            if (seconds < 3600) return seconds / 60 + " menit lalu";
            if (seconds < 86400) return seconds / 3600 + " jam lalu";
            return seconds / 86400 + " hari lalu";
        }

        public static string Graphics(string profile) => "Grafis: " + profile + "  ›";

        public static string Loading(float progress) =>
            "Memuat Sukabaja… " + (int)Math.Round(100 * Math.Min(1, Math.Max(0, progress))) + "%";
    }
}
