using System.Collections.Generic;
using System.Linq;
using Steel.Services;

namespace Steel.Presentation.UI
{
    /// <summary>Buku usaha page for the season calendar and Grup Usaha (PRD 11.3).</summary>
    internal sealed partial class YardDesk
    {
        private readonly List<string> legacyPicks = new List<string>();
        private int seasonPast, legacyShown;

        private void DrawSeason()
        {
            feedback.text = networkMessage;
            if (networkBusy) { detail.text = "Menunggu jawaban " + network.Host + "…"; return; }
            var board = network.Board;
            if (board == null) { detail.text = "Hubungkan akun di halaman Pasar jaringan untuk melihat season."; return; }
            if (network.HasPending)
            {
                detail.text = "Jawaban pasar belum diterima. Periksa hasilnya dengan ID pesanan yang sama.";
                Key(0,"Periksa pesanan",() => NetworkRun(network.Retry));
                return;
            }
            var group = board.group;
            if (forms && group.choose) { DrawLegacies(group); return; }
            detail.text = "Season " + board.season + "   ·   Kota " + board.city + "   ·   Hari " + board.seasonDay + "/" +
                NetworkGroup.SeasonDays + "\n" + (board.closing
                    ? "PEKAN TUTUP BUKU: " + board.seasonDaysLeft + " hari lagi. Kontrak baru harus selesai sebelum tutup buku."
                    : "Tutup buku " + board.seasonDaysLeft + " hari lagi. Usaha di kota ini lalu masuk sejarah grup.") +
                "\nGrup Usaha level " + group.level + "   ·   " + group.points + " Poin Grup" +
                (group.picks.Length > 0 ? "   ·   Bekal: " + string.Join(", ",group.picks.Select(id => LegacyName(group,id))) : "");
            if (group.history.Length > 0)
            {
                seasonPast %= group.history.Length;
                var past = group.history[seasonPast];
                detail.text += "\n" + past.Title + "   ·   nilai " + Rp(past.value) + "   ·   +" + past.points + " poin";
                if (group.history.Length > 1)
                    Key(1,"Kota sebelumnya",() => seasonPast = (seasonPast + 1) % group.history.Length);
            }
            if (group.choose) Key(0,"Pilih bekal",() => { forms = true; legacyPicks.Clear(); networkMessage = ""; });
            else if (group.picks.Length > 0) DrawLegacyUse(group);
            Key(2,"Muat ulang",() => NetworkRun(network.Refresh));
        }

        private static string LegacyName(NetworkGroup group, string id) =>
            group.legacies.FirstOrDefault(l => l.id == id)?.name ?? id;

        private void DrawLegacies(NetworkGroup group)
        {
            legacyShown %= group.legacies.Length;
            var legacy = group.legacies[legacyShown];
            bool open = legacy.level <= group.level, taken = legacyPicks.Contains(legacy.id);
            detail.text = "Bekal warisan " + (legacyShown + 1) + "/" + group.legacies.Length + "   ·   " + legacy.name +
                "\n" + (open ? taken ? "Dipilih." : "Terbuka." : "Terbuka di grup level " + legacy.level + ".") +
                "\nDipilih " + legacyPicks.Count + "/" + NetworkGroup.Picks + ": " +
                string.Join(", ",legacyPicks.Select(id => LegacyName(group,id))) + "\nPilihan berlaku sekali untuk season ini.";
            if (taken) Key(0,"Lepas",() => legacyPicks.Remove(legacy.id));
            else if (open && legacyPicks.Count < NetworkGroup.Picks) Key(0,"Ambil",() => legacyPicks.Add(legacy.id));
            Key(1,"Bekal berikut",() => legacyShown = (legacyShown + 1) % group.legacies.Length);
            if (legacyPicks.Count == 0) Key(2,"Kembali",() => forms = false);
            else Key(2,"Simpan " + legacyPicks.Count + " bekal",() =>
            {
                forms = false;
                NetworkRun(() => network.PickLegacies(legacyPicks.ToArray()));
            });
        }
    }
}
