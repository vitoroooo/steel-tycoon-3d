using System;
using System.Globalization;
using Steel.Presentation.Input;
using Steel.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    /// <summary>The screen at the yard's loading post and at the mill's weighing post: one truck, what is on it, what to do next.</summary>
    internal sealed class HaulDesk
    {
        private readonly IGameService game;
        private readonly SliceInput input;
        private readonly CityHud hud;
        private readonly RectTransform panel;
        private readonly Text title, truck, status, voice;
        private readonly Button[] grades = new Button[4];
        private readonly Button stop, weigh, orders;
        private readonly SpkDesk spk;
        private bool atGate;
        internal bool Opened => panel.gameObject.activeSelf;
        private static string Rp(double v) => "Rp" + v.ToString("N0", CultureInfo.GetCultureInfo("id-ID"));
        private static string Ton(double kg) => (kg / 1000).ToString("0.0", CultureInfo.GetCultureInfo("id-ID")) + " t";

        internal HaulDesk(CityHud hud, IGameService game, SliceInput input)
        {
            this.hud = hud; this.game = game; this.input = input;
            panel = UiFactory.Panel("Pos truk", hud.SafeRoot, new Vector2(.5f, .5f), new Vector2(800, 430), Vector2.zero);
            title = Line(180, 24, 40); truck = Line(138, 19, 34); status = Line(84, 18, 60);
            voice = Line(24, 18, 44); voice.color = UiFactory.Gold;
            for (int i = 0; i < grades.Length; i++)
            {
                int grade = i;
                grades[i] = Key("G" + i, 182, -285 + i * 190, -62, () => game.LoadTruck(grade));
            }
            stop = Key("BERHENTI MUAT", 300, 125, -160, game.StopLoading);
            weigh = Key("TIMBANG & BONGKAR", 340, 105, -62, () => voice.text = "Nota timbang: " + Rp(game.WeighIn()) + " dibayar setelah bongkar.");
            spk = new SpkDesk(hud.SafeRoot, game);
            orders = UiFactory.Button(panel, "KONTRAK SPK", new Vector2(230, 64), new Vector2(-55, -160), spk.Open);
            UiFactory.Button(panel, "TUTUP", new Vector2(182, 64), new Vector2(-285, -160), Close);
            panel.gameObject.AddComponent<Ticker>().Every = Refresh;
            panel.gameObject.SetActive(false);
        }

        private Text Line(int y, int size, int height) => UiFactory.Text(panel, "", size, new Vector2(740, height), new Vector2(0, y));

        private Button Key(string caption, int width, int x, int y, Action press) =>
            UiFactory.Button(panel, caption, new Vector2(width, 64), new Vector2(x, y), () =>
            {
                try { press(); Refresh(); }
                catch (InvalidOperationException e) { voice.text = e.Message; }
            });

        internal void Open(bool gate)
        {
            atGate = gate; voice.text = "";
            panel.gameObject.SetActive(true); input.IsBlocked = true; Refresh();
        }

        private void Close() { spk.Close(); panel.gameObject.SetActive(false); input.IsBlocked = false; hud.RefreshNow(); }

        private void Refresh()
        {
            HaulView v = atGate ? game.GateDock : game.YardDock;
            title.text = atGate ? "POS TIMBANG · PT BAJA NUSANTARA" : "POS MUAT · LAPAK ANDA";
            truck.text = v.Truck == null ? "Belum ada truk Anda di sini. Parkir engkel, tronton atau trailer lalu kembali."
                : v.TruckName.ToUpperInvariant() + "  ·  muatan " + Ton(v.LoadKg) + " / " + Ton(v.CapacityKg) + (v.Grade == null ? "" : "  ·  " + v.Grade);
            status.text = v.Loading ? "Loader memuat sampai " + Ton(v.TargetKg) + ", sisa " + Minutes(v.HoursLeft)
                : v.Unloading ? "Crane membongkar, sisa " + Minutes(v.HoursLeft) + "\nDibayar " + Rp(v.Net) + " setelah selesai"
                : v.LoadKg > 0 ? (v.SpkKg > 0 ? Ton(v.SpkKg) + " masuk SPK" : "Harga harian " + Rp(v.PerKg) + "/kg") + Queue() + "\nBersih " + Rp(v.Net) + " setelah biaya jalan " + Rp(v.RunCost)
                : v.Notice ?? (atGate ? "Bak kosong. Muat dulu di kavling lapak Anda." : "Pilih grade yang dimuat. Loader bekerja 10 ton per jam game.");
            for (int i = 0; i < grades.Length; i++)
            {
                grades[i].gameObject.SetActive(!atGate);
                grades[i].GetComponentInChildren<Text>().text = game.YardInfo(i).Grade + "\n" + Ton(v.Stock[i]);
            }
            stop.gameObject.SetActive(!atGate && v.Loading); weigh.gameObject.SetActive(atGate); orders.gameObject.SetActive(atGate);
        }

        private string Queue() => !atGate ? "" : game.GateQueue == 0 ? " · gerbang kosong"
            : " · antre " + game.GateQueue + " truk (" + game.GateQueue * 10 + " menit game)";

        private static string Minutes(double gameHours) => Math.Ceiling(gameHours * 60) + " menit game";
    }
}
