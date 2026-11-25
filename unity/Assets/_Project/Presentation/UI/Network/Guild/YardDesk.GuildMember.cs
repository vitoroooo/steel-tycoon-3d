using System;
using System.Linq;
using Steel.Services;

namespace Steel.Presentation.UI
{
    /// <summary>What a member sees of his association: the chat, the blockade desk, and the member list.</summary>
    internal sealed partial class YardDesk
    {
        private void DrawGuildMember(NetworkBoard board)
        {
            var guild = board.association;
            detail.text = guild.name + "   ·   " + guild.members.Length + "/" + NetworkGuild.MaxMembers +
                " anggota   ·   Kasta " + board.caste;
            if (guildMode == 1) { DrawGuildBlockade(board); return; }
            if (guildMode == 2)
            {
                detail.text += "\n" + string.Join(", ",guild.members);
                Key(0,"Keluar asosiasi",() => NetworkRun(network.Leave));
                Key(1,"Muat ulang",() => NetworkRun(network.Refresh));
                Key(2,"Menu: chat",() => guildMode = 0);
                return;
            }
            var latest = guild.messages.Skip(Math.Max(0,guild.messages.Length - 2)).Select(m => m.name + ": " + m.text);
            detail.text += "\n" + (guild.messages.Length == 0 ? "Belum ada pesan." : string.Join("\n",latest));
            Key(0,"Tulis pesan",() => GuildField("Pesan untuk " + guild.name,NetworkGuild.ChatLength,
                text => NetworkRun(() => network.Say(text))));
            var other = guild.messages.LastOrDefault(m => m.name != board.name);
            if (other != null) Key(1,"Laporkan " + other.name,() => NetworkRun(() => network.Report(other.id)));
            else Key(1,"Muat ulang",() => NetworkRun(network.Refresh));
            Key(2,"Menu: blokade",() => guildMode = 1);
        }

        private void DrawGuildBlockade(NetworkBoard board)
        {
            Key(2,"Menu: anggota",() => guildMode = 2);
            if (board.caste < 3)
            {
                long cost = NetworkGuild.PromoteCost[board.caste - 1];
                detail.text += "\nBlokade rute hanya untuk anggota Kasta 3 terhadap pesaing Kasta 3." +
                    "\nPaket naik Kasta " + (board.caste + 1) + ": " + Rp(cost) + " dari kas pasar.";
                if (board.cash >= cost) Key(0,"Naik Kasta " + (board.caste + 1),() => NetworkRun(network.Promote));
                return;
            }
            if (board.rivals.Length == 0) { detail.text += "\nBelum ada pesaing Kasta 3 di luar asosiasi Anda."; return; }
            guildRival %= board.rivals.Length;
            string rival = board.rivals[guildRival];
            detail.text += "\nBlokade rute " + rival + " selama 1 hari game   ·   " + Rp(NetworkGuild.BlockadeCost) +
                "\nJeda 3 hari game per target. Pemain baru Kasta 3 dilindungi 7 hari.";
            if (board.cash >= NetworkGuild.BlockadeCost)
                Key(0,"Blokade " + rival,() => NetworkRun(() => network.Blockade(rival)));
            else detail.text += "\nKas pasar kurang.";
            if (board.rivals.Length > 1) Key(1,"Target berikut",() => guildRival = (guildRival + 1) % board.rivals.Length);
        }
    }
}
