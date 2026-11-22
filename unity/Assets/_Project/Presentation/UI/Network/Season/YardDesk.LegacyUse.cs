using System;
using System.Linq;
using Steel.Services;

namespace Steel.Presentation.UI
{
    /// <summary>Lets a veteran start this career with the legacies picked for the season (PRD 11.3).</summary>
    internal sealed partial class YardDesk
    {
        private void DrawLegacyUse(NetworkGroup group)
        {
            if (game.Legacies.Length > 0)
                detail.text += "\nKarier ini berjalan dengan: " + string.Join(", ",game.Legacies.Select(LegacyText.Effect)) + ".";
            else if (!game.LegacyOpen)
                detail.text += "\nBekal hanya bisa dipakai di karier baru sebelum penjualan pertama (menu awal, Karier baru).";
            else Key(0,"Pakai bekal",() =>
            {
                try { networkMessage = game.StartWithLegacies(group.picks); }
                catch (InvalidOperationException e) { networkMessage = e.Message; }
            });
        }
    }
}
