using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Steel.Services;
using Steel.Presentation.Input;

namespace Steel.Presentation.UI
{
    internal sealed class RouteDesk
    {
        private readonly IGameService game;
        private readonly SliceInput input;
        private readonly RectTransform panel;
        private readonly Text status, title, quote;
        private readonly Text[] options = new Text[3];
        private int vehicle, district;
        private bool purchase;
        private RouteView offer;
        private float nextRefresh;
        private static readonly string[] Ids = { "cart", FleetCatalog.Triseda, FleetCatalog.Pikap };
        private static readonly string[] Labels = { "Gerobak", "Motor bak", "Pikap" };
        private static string Rp(double v) => "Rp" + v.ToString("N0", CultureInfo.GetCultureInfo("id-ID"));
        internal bool Opened => panel.gameObject.activeSelf;

        internal RouteDesk(CityHud hud, IGameService service, SliceInput controls, Action openMarket)
        {
            game = service; input = controls;
            panel = UiFactory.Panel("Meja rute", hud.SafeRoot, new Vector2(.5f, .5f), new Vector2(680, 360), Vector2.zero);
            title = Line(145, 23); title.text = "LAPAK  /  Jadwal pengumpulan";
            UiFactory.Button(panel, "PAPAN HARGA", new Vector2(164, 46), new Vector2(227, 139),
                () => { Close(); openMarket(); });
            quote = Line(73, 18); status = Line(-44, 16);
            for (int i = 0; i < 3; i++)
            {
                int key = i;
                var btn = UiFactory.Button(panel, "", new Vector2(200, 48), new Vector2((i - 1) * 212, 4),
                    () => Select(key));
                options[i] = btn.GetComponentInChildren<Text>();
            }
            UiFactory.Button(panel, "KEMBALI", new Vector2(134, 52), new Vector2(-249, -127), Close);
            UiFactory.Button(panel, "AMBIL KIRIMAN", new Vector2(190, 52), new Vector2(-72, -127),
                () => Try(() => "Diambil: " + game.CollectRoute().ToString("N0") + " kg. Sortir sebelum dijual."));
            UiFactory.Button(panel, "PESAN RUTE", new Vector2(202, 52), new Vector2(207, -127),
                () => Try(() => { game.BookRoute(offer); return "Armada terjadwal. Hasil kembali ke lapak."; }));
            panel.gameObject.SetActive(false);
        }
        private Text Line(float y, int size) => UiFactory.Text(panel, "", size, new Vector2(630, 63),
            new Vector2(0, y), TextAnchor.MiddleLeft);
        private void Select(int key)
        {
            if (key == 0) { vehicle = (vehicle + 1) % 3; district = 0; }
            if (key == 1) district = (district + 1) % (vehicle + 1);
            if (key == 2) purchase = !purchase;
            Refresh();
        }
        internal void Open()
        {
            if (!game.MotorRoute && game.VehicleOf(FleetCatalog.Triseda).Owned) { vehicle = 1; district = 0; }
            panel.gameObject.SetActive(true); input.IsBlocked = true; Refresh();
        }
        private void Close() { panel.gameObject.SetActive(false); input.IsBlocked = false; }
        private void Try(Func<string> act)
        {
            try { string result = act(); Refresh(); status.text = result + "\n" + status.text; }
            catch (InvalidOperationException e) { Refresh(); status.text = e.Message + "\n" + status.text; }
            nextRefresh = Time.unscaledTime + 5;
        }
        internal void Tick()
        {
            if (Opened && Time.unscaledTime >= nextRefresh) Refresh();
        }
        private void Refresh()
        {
            nextRefresh = Time.unscaledTime + 1;
            offer = game.Route(Ids[vehicle], district, purchase);
            options[0].text = Labels[vehicle] + "  ›"; options[1].text = offer.Area + "  ›";
            options[2].text = purchase ? "Beli dari NPC  ›" : "Pungut gratis  ›";
            quote.text = offer.Kg + " kg · " + (purchase ? "HMS 1" : "Rongsok campur") + " · " +
                offer.Minutes + " menit nyata\nBarang " + Rp(offer.Goods) + " + jalan " + Rp(offer.Travel) +
                " = " + Rp(offer.Goods + offer.Travel);
            status.text = "Antrean: " + offer.Queue + (offer.Queue > 0 ? " · Tiba " +
                Math.Ceiling(offer.ArrivalMinutes) + " menit lagi" : "") + "   |   Tersedia di lapak: " + offer.DepotKg + " kg";
        }
    }
}
