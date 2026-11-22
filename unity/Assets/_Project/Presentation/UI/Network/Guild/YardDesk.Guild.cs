using Steel.Services;

namespace Steel.Presentation.UI
{
    /// <summary>Buku usaha page for associations (PRD 8.1) and route blockades (PRD 6.3).</summary>
    internal sealed partial class YardDesk
    {
        private int guildMode, guildShown, guildRival;

        private void DrawGuild()
        {
            feedback.text = networkMessage;
            if (networkBusy) { detail.text = "Menunggu jawaban " + network.Host + "…"; return; }
            var board = network.Board;
            if (board == null) { detail.text = "Hubungkan akun di halaman Pasar jaringan untuk masuk asosiasi."; return; }
            if (network.HasPending)
            {
                detail.text = "Jawaban pasar belum diterima. Periksa hasilnya dengan ID pesanan yang sama.";
                Key(0,"Periksa pesanan",() => NetworkRun(network.Retry));
                return;
            }
            if (board.Blocked && !forms)
            {
                detail.text = "Rute Anda diblokade suruhan " + board.blockedBy + " sampai jam kota " + board.blockedUntil +
                    " (sekarang " + board.gameHour + ").\nLot dan kiriman kontrak tertahan sampai rute terbuka.";
                if (board.cash >= NetworkGuild.EscortCost)
                    Key(0,"Pengawal " + Rp(NetworkGuild.EscortCost),() => NetworkRun(network.Escort));
                else detail.text += "\nKas pasar kurang untuk menyewa pengawalan " + Rp(NetworkGuild.EscortCost) + ".";
                Key(2,"Lanjut",() => forms = true);
                return;
            }
            if (board.association.Joined) DrawGuildMember(board); else DrawGuildList(board);
        }

        private void DrawGuildList(NetworkBoard board)
        {
            Key(2,"Dirikan asosiasi",() => GuildField("Nama asosiasi baru",24,
                name => NetworkRun(() => network.Found(name))));
            if (board.associations.Length == 0)
            {
                detail.text = "Belum ada asosiasi di kota ini. Dirikan yang pertama (maksimal " +
                    NetworkGuild.MaxMembers + " anggota).";
                Key(0,"Muat ulang",() => NetworkRun(network.Refresh));
                return;
            }
            guildShown %= board.associations.Length;
            var entry = board.associations[guildShown];
            detail.text = "Asosiasi " + (guildShown + 1) + "/" + board.associations.Length + "   ·   " + entry.name +
                "   ·   " + entry.members + "/" + NetworkGuild.MaxMembers + " anggota" +
                "\nAnggota berbagi chat; anggota Kasta 3 dapat menyewa blokade rute pesaing.";
            if (entry.members < NetworkGuild.MaxMembers) Key(0,"Gabung",() => NetworkRun(() => network.Join(entry.name)));
            else detail.text += "\nAsosiasi ini sudah penuh.";
            if (board.associations.Length > 1)
                Key(1,"Asosiasi berikut",() => guildShown = (guildShown + 1) % board.associations.Length);
        }
    }
}
