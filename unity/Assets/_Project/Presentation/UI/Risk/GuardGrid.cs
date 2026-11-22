using System;
using System.Globalization;
using Steel.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    /// <summary>The two front rows of the 5x4 gate grid on the security desk: pick a kind of guard, tap an empty post to hire,
    /// tap a guard to send him home. With a gang at the gate an empty post hires the escort team instead.</summary>
    internal sealed class GuardGrid
    {
        private const int Columns = 5, Posts = 10, Escort = 6;
        internal static readonly string[] Names = { "·", "JAGA", "PENTUNG", "ANJING", "CCTV", "NEGOSIATOR", "PENGAWAL" };
        private readonly Button[] posts = new Button[Posts];
        private readonly Text pick;
        private int kind = 1;

        internal GuardGrid(RectTransform panel, IGameService game, Action<Action> run)
        {
            UiFactory.Text(panel, "REGU JAGA GERBANG · jalan di atas, preman menyerbu dari tengah", 16, new Vector2(520, 24), new Vector2(98, 34));
            for (int cell = 0; cell < posts.Length; cell++)
            {
                int at = cell;
                posts[cell] = UiFactory.Button(panel, "Pos " + cell, new Vector2(100, 64),
                    new Vector2(-110 + cell % Columns * 104, -18 - cell / Columns * 70),
                    () => run(() => { RiskView risk = game.Risk; game.PostGuard(at, risk.Guards[at] != 0 ? 0 : risk.Demand ? Escort : kind); }));
            }
            pick = UiFactory.Button(panel, "Jenis satpam", new Vector2(230, 92), new Vector2(-310, -52),
                () => run(() => kind = kind % (Escort - 1) + 1)).GetComponentInChildren<Text>();
            UiFactory.Text(panel, "Ketuk pos kosong\nuntuk merekrut", 15, new Vector2(230, 44), new Vector2(-310, -126));
        }

        internal void Refresh(RiskView risk)
        {
            int hire = risk.Demand ? Escort : kind;
            pick.text = (risk.Demand ? "SEWA: " : "REKRUT: ") + Names[hire] + "\nRp" +
                risk.GuardCost[hire].ToString("N0", CultureInfo.GetCultureInfo("id-ID"));
            for (int cell = 0; cell < posts.Length; cell++)
            {
                posts[cell].GetComponentInChildren<Text>().text = Names[risk.Guards[cell]] + (risk.Fainted[cell] ? "\n(pingsan)" : "");
                posts[cell].GetComponent<Image>().color = risk.Guards[cell] == 0 ? new Color(.3f, .34f, .4f)
                    : risk.Fainted[cell] ? new Color(.55f, .45f, .3f) : UiFactory.Gold;
            }
        }
    }
}
