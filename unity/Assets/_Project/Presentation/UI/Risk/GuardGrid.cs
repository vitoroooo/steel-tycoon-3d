using System;
using System.Globalization;
using Steel.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    /// <summary>The 5x4 gate grid on the security desk: pick a kind of guard, tap an empty post to hire, tap a guard to send him home.</summary>
    internal sealed class GuardGrid
    {
        private const int Lanes = 4, Columns = 5;
        internal static readonly string[] Names = { "·", "TAMENG", "PENTUNG", "PENJINAK" };
        private readonly IGameService game;
        private readonly Button[] posts = new Button[Lanes * Columns];
        private readonly Text pick;
        private int kind = 1;

        internal GuardGrid(RectTransform panel, IGameService game, Action<Action> run)
        {
            this.game = game;
            UiFactory.Text(panel, "REGU JAGA GERBANG · 4 lajur, jalan di kanan", 16, new Vector2(520, 24), new Vector2(98, 34));
            for (int cell = 0; cell < posts.Length; cell++)
            {
                int at = cell;
                posts[cell] = UiFactory.Button(panel, "Pos " + cell, new Vector2(100, 44),
                    new Vector2(-110 + cell % Columns * 104, -4 - cell / Columns * 48),
                    () => run(() => game.PostGuard(at, game.Risk.Guards[at] == 0 ? kind : 0)));
            }
            pick = UiFactory.Button(panel, "Jenis satpam", new Vector2(230, 92), new Vector2(-310, -52),
                () => run(() => kind = kind % (Names.Length - 1) + 1)).GetComponentInChildren<Text>();
            UiFactory.Text(panel, "Ketuk pos kosong\nuntuk merekrut", 15, new Vector2(230, 44), new Vector2(-310, -126));
        }

        internal void Refresh(RiskView risk)
        {
            pick.text = "REKRUT: " + Names[kind] + "\nRp" + risk.GuardCost[kind].ToString("N0", CultureInfo.GetCultureInfo("id-ID"));
            for (int cell = 0; cell < posts.Length; cell++)
            {
                posts[cell].GetComponentInChildren<Text>().text = Names[risk.Guards[cell]];
                posts[cell].GetComponent<Image>().color = risk.Guards[cell] == 0 ? new Color(.3f, .34f, .4f) : UiFactory.Gold;
            }
        }
    }
}
