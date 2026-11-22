using System;
using System.Globalization;
using Steel.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    /// <summary>The yard's security desk: heat and raids, today's shady offer, the gang at the gate, and the guards on the grid.</summary>
    internal sealed class SecurityDesk
    {
        private static readonly string[] GangKeys = { "BAYAR", "NEGOSIASI", "BERTAHAN" }, OfferKeys = { "TERIMA", "TOLAK", "LAPORKAN" };
        private readonly IGameService game;
        private readonly RectTransform panel;
        private readonly Text status, happening, voice, auto, protector;
        private readonly Button[] answers = new Button[3];
        private readonly GuardGrid grid;
        private static readonly CultureInfo Id = CultureInfo.GetCultureInfo("id-ID");
        private static string Rp(double v) => "Rp" + v.ToString("N0", Id);

        internal SecurityDesk(Transform parent, IGameService game)
        {
            this.game = game;
            panel = UiFactory.Panel("Pos keamanan", parent, new Vector2(.5f, .5f), new Vector2(900, 560), Vector2.zero);
            UiFactory.Text(panel, "KEAMANAN LAPAK", 24, new Vector2(840, 36), new Vector2(0, 252));
            status = UiFactory.Text(panel, "", 17, new Vector2(840, 28), new Vector2(0, 216));
            happening = UiFactory.Text(panel, "", 18, new Vector2(840, 56), new Vector2(0, 168));
            for (int i = 0; i < answers.Length; i++)
            {
                int choice = i;
                answers[i] = UiFactory.Button(panel, "Jawab " + i, new Vector2(270, 56), new Vector2(-290 + i * 290, 100),
                    () => Run(() => voice.text = game.Risk.Demand ? game.AnswerGang(choice) : game.AnswerContraband(choice)));
            }
            grid = new GuardGrid(panel, game, Run);
            voice = UiFactory.Text(panel, "", 16, new Vector2(840, 26), new Vector2(0, -180)); voice.color = UiFactory.Gold;
            auto = UiFactory.Button(panel, "Jawaban otomatis", new Vector2(280, 56), new Vector2(-295, -232),
                () => Run(game.ToggleAutoDefend)).GetComponentInChildren<Text>();
            protector = UiFactory.Button(panel, "Pelindung", new Vector2(290, 56), new Vector2(0, -232),
                () => Run(game.HireProtector)).GetComponentInChildren<Text>();
            UiFactory.Button(panel, "KEMBALI", new Vector2(200, 56), new Vector2(300, -232), Close);
            panel.gameObject.AddComponent<Ticker>().Every = Refresh;
            panel.gameObject.SetActive(false);
        }

        internal void Open() { voice.text = ""; panel.gameObject.SetActive(true); panel.SetAsLastSibling(); Refresh(); }
        internal void Close() => panel.gameObject.SetActive(false);

        private void Run(Action press)
        {
            try { press(); }
            catch (InvalidOperationException e) { voice.text = e.Message; }
            Refresh();
        }

        private void Refresh()
        {
            RiskView risk = game.Risk;
            status.text = "Sorotan " + risk.Heat.ToString("0") + " · peluang razia " + (risk.RaidChance * 100).ToString("0") + "% per muatan gelap" +
                (risk.QuietHours > 0 ? " · preman tenang " + Math.Ceiling(risk.QuietHours) + " jam game" : "");
            happening.text = risk.Demand
                ? risk.Thugs + " PREMAN DI GERBANG minta " + Rp(risk.DemandDue) + " (2% omzet bulan ini)\nMenunggu " +
                  Math.Ceiling(risk.DemandHoursLeft) + " jam game, lalu dijawab otomatis"
                : risk.Offer ? "TAWARAN GELAP: " + risk.OfferName + " " + risk.OfferKg.ToString("N0", Id) + " kg\nLolos: untung " +
                  Rp(risk.OfferGain) + " · Razia: disita, denda " + Rp(risk.OfferFine) + ", reputasi -25"
                : risk.Note ?? "Gerbang aman. Tidak ada tawaran hari ini.";
            for (int i = 0; i < answers.Length; i++)
            {
                answers[i].gameObject.SetActive(risk.Demand || risk.Offer);
                answers[i].GetComponentInChildren<Text>().text = risk.Demand
                    ? GangKeys[i] + (i == 1 ? risk.Haggled ? " (GAGAL)" : " JADI 1%" : "") : OfferKeys[i];
            }
            answers[1].interactable = !risk.Demand || !risk.Haggled;
            auto.text = "TAK DIJAWAB: " + (risk.AutoDefend ? "BERTAHAN" : "BAYAR");
            protector.text = risk.ProtectorHours > 0 ? "PELINDUNG " + Math.Ceiling(risk.ProtectorHours) + " JAM"
                : "PELINDUNG " + Rp(risk.ProtectorFee) + "/bln";
            grid.Refresh(risk);
        }
    }
}
