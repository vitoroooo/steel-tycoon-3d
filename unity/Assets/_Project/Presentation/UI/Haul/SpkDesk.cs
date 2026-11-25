using System;
using System.Globalization;
using Steel.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    /// <summary>The mill's work-order board at its weighing post: today's three orders, and the one being filled.</summary>
    internal sealed class SpkDesk
    {
        private readonly IGameService game;
        private readonly RectTransform panel;
        private readonly Text running, voice;
        private readonly Text[] lines = new Text[3];
        private readonly Button[] take = new Button[3];
        private static readonly CultureInfo Id = CultureInfo.GetCultureInfo("id-ID");
        private static string Rp(double v) => "Rp" + v.ToString("N0", Id);

        internal SpkDesk(Transform parent, IGameService game)
        {
            this.game = game;
            panel = UiFactory.Panel("Papan SPK", parent, new Vector2(.5f, .5f), new Vector2(860, 470), Vector2.zero);
            UiFactory.Text(panel, "SPK · PT BAJA NUSANTARA", 24, new Vector2(800, 40), new Vector2(0, 200));
            running = UiFactory.Text(panel, "", 18, new Vector2(800, 64), new Vector2(0, 142));
            for (int i = 0; i < lines.Length; i++)
            {
                int slot = i;
                lines[i] = UiFactory.Text(panel, "", 17, new Vector2(590, 60), new Vector2(-105, 66 - i * 72));
                lines[i].alignment = TextAnchor.MiddleLeft;
                take[i] = UiFactory.Button(panel, "AMBIL", new Vector2(170, 60), new Vector2(310, 66 - i * 72), () => Take(slot));
            }
            voice = UiFactory.Text(panel, "", 17, new Vector2(800, 40), new Vector2(0, -150)); voice.color = UiFactory.Gold;
            UiFactory.Button(panel, "KEMBALI", new Vector2(190, 60), new Vector2(0, -200), () => panel.gameObject.SetActive(false));
            panel.gameObject.AddComponent<Ticker>().Every = Refresh;
            panel.gameObject.SetActive(false);
        }

        internal void Open() { voice.text = ""; panel.gameObject.SetActive(true); panel.SetAsLastSibling(); Refresh(); }
        internal void Close() => panel.gameObject.SetActive(false);

        private void Take(int slot)
        {
            try { game.TakeSpk(slot); voice.text = "SPK diambil. Jaminan dikunci sampai kiriman lunas."; }
            catch (InvalidOperationException e) { voice.text = e.Message; }
            Refresh();
        }

        private void Refresh()
        {
            SpkView spk = game.Spk; SpkOfferView[] board = game.SpkBoard;
            running.text = spk.Active
                ? "Berjalan: " + spk.Grade + " " + spk.DeliveredTonnes.ToString("0.0", Id) + " / " + spk.Tonnes.ToString("0", Id) + " ton @ " +
                  Rp(spk.PerKg) + "/kg\n" + (spk.HoursLeft >= 0 ? "Tenggat " + Math.Ceiling(spk.HoursLeft) + " jam game lagi"
                      : "TERLAMBAT " + Math.Ceiling(-spk.HoursLeft) + " jam · denda 2% per jam") + " · jaminan " + Rp(spk.Bond)
                : (spk.Note ?? "Belum ada SPK berjalan.") + "\nReputasi Anda " + spk.Reputation;
            for (int i = 0; i < lines.Length; i++)
            {
                SpkOfferView o = board[i];
                lines[i].text = o.Grade + " · " + o.Tonnes.ToString("0", Id) + " ton · " + Rp(o.PerKg) + "/kg · " + o.Days +
                    " hari game\nJaminan " + Rp(o.Bond) + " · reputasi min. " + o.MinReputation;
                take[i].interactable = !spk.Active && !o.Taken;
                take[i].GetComponentInChildren<Text>().text = o.Taken ? "DIAMBIL" : "AMBIL";
            }
        }
    }
}
