using System;
using System.Globalization;
using Steel.Presentation.Input;
using Steel.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    internal sealed class MarketBoard
    {
        private readonly IGameService game;
        private readonly SliceInput input;
        private readonly RectTransform panel;
        private readonly Text name, rates, receipt, submit;
        private readonly CityHud hud;
        private int cursor;
        private bool quoted;
        internal bool Opened => panel.gameObject.activeSelf;
        private static string Rp(double v) => "Rp" + v.ToString("N0", CultureInfo.GetCultureInfo("id-ID"));
        internal MarketBoard(CityHud hud, IGameService game, SliceInput input)
        {
            this.hud = hud; this.game = game; this.input = input;
            panel = UiFactory.Panel("Papan harga lokal", hud.SafeRoot, new Vector2(.5f,.5f), new Vector2(752,402), Vector2.zero);
            Line(171, 18, 30).text = "SUKABAJA   /   PAPAN HARGA BELI";
            name = Line(113, 24, 55); rates = Line(34, 19, 88); receipt = Line(-66, 18, 86);
            UiFactory.Button(panel, "TUTUP", new Vector2(124,50), new Vector2(-285,-153), Close);
            UiFactory.Button(panel, "PEMBELI BERIKUT", new Vector2(198,50), new Vector2(-99,-153),
                () => { cursor = (cursor + 1) % game.BuyerCount; Refresh(); });
            submit = UiFactory.Button(panel, "PERIKSA NOTA", new Vector2(265,50), new Vector2(191,-153),
                Submit).GetComponentInChildren<Text>();
            panel.gameObject.SetActive(false);
        }
        private Text Line(int y, int size, int height) => UiFactory.Text(panel, "", size, new Vector2(690,height),
            new Vector2(0,y));
        internal void Open()
        {
            cursor = 0;
            for (int i = 0; i < game.BuyerCount; i++) if (game.Buyer(i).AtCounter) cursor = i;
            panel.gameObject.SetActive(true); input.IsBlocked = true; Refresh();
        }
        private void Close() { panel.gameObject.SetActive(false); input.IsBlocked = false; quoted = false; }
        private void Refresh()
        {
            var b = game.Buyer(cursor); quoted = false; submit.text = "PERIKSA NOTA";
            name.text = b.Name;
            rates.text = "Rp/kg bersih   ·   Harga bandar/toko NPC\n" + b.Rates;
            receipt.text = b.AtCounter ? "Di depan pembeli · Tanpa ongkir" :
                Math.Ceiling(b.Distance) + " m ke " + b.Direction + " · Antar sendiri tanpa ongkir\n" +
                "Kirim dari lapak: " + Rp(b.FreightPerKg) + "/kg bruto";
        }
        private void Submit()
        {
            try
            {
                if (quoted)
                {
                    double net = game.SellLocal(); Close(); hud.NoticeText.text = "Kas bertambah " + Rp(net); return;
                }
                var n = game.InspectBid(cursor); quoted = true;
                receipt.text = n.Kg.ToString("0.##") + " kg bersih · Bruto " + Rp(n.Gross) + " − ongkir " + Rp(n.Freight) +
                    "\nKas masuk " + Rp(n.Net) + " · Margin " + Rp(n.Margin);
                submit.text = n.Shipped ? "KONFIRMASI KIRIM" : "KONFIRMASI JUAL";
            }
            catch (InvalidOperationException e) { Refresh(); receipt.text = e.Message; }
        }
    }
}
