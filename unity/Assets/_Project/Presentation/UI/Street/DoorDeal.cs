using System;
using System.Globalization;
using Steel.Presentation.Input;
using Steel.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    /// <summary>The screen for buying scrap at a door: look at the goods, weigh them, bargain, pay. Sized for thumbs.</summary>
    internal sealed class DoorDeal
    {
        private readonly IGameService game;
        private readonly SliceInput input;
        private readonly CityHud hud;
        private readonly RectTransform panel;
        private readonly Text title, goods, price, bill, voice;
        private string id;
        private double offer;
        internal bool Opened => panel.gameObject.activeSelf;
        private static string Rp(double v) => "Rp" + v.ToString("N0", CultureInfo.GetCultureInfo("id-ID"));

        internal DoorDeal(CityHud hud, IGameService game, SliceInput input)
        {
            this.hud = hud; this.game = game; this.input = input;
            panel = UiFactory.Panel("Beli di tempat", hud.SafeRoot, new Vector2(.5f, .5f), new Vector2(780, 430), Vector2.zero);
            title = Line(180, 24, 40); goods = Line(136, 19, 34); price = Line(98, 19, 34); bill = Line(52, 18, 50);
            voice = Line(-2, 19, 44); voice.color = UiFactory.Gold;
            Key("LIHAT BARANG", 178, -278, -76, () => game.InspectSeller(id));
            Key("TIMBANG", 138, -112, -76, () => game.WeighSeller(id));
            Key("−", 70, 0, -76, () => offer = Math.Max(game.Seller(id).Step, offer - game.Seller(id).Step));
            Key("+", 70, 78, -76, () => offer += game.Seller(id).Step);
            Key("TAWAR", 176, 209, -76, Haggle);
            UiFactory.Button(panel, "TUTUP", new Vector2(178, 64), new Vector2(-278, -160), Close);
            Key("BAYAR & MUAT", 330, 132, -160, Pay);
            panel.gameObject.SetActive(false);
        }

        private Text Line(int y, int size, int height) =>
            UiFactory.Text(panel, "", size, new Vector2(720, height), new Vector2(0, y));

        /// <summary>A button whose refusal, if any, is spoken on the panel instead of being lost.</summary>
        private void Key(string caption, int width, int x, int y, Action press) =>
            UiFactory.Button(panel, caption, new Vector2(width, 64), new Vector2(x, y), () =>
            {
                try { press(); if (Opened) Refresh(); }
                catch (InvalidOperationException e) { Refresh(); voice.text = e.Message; }
            });

        internal void Open(string seller)
        {
            id = seller; offer = game.Seller(id).AskPerKg;
            voice.text = "\"Ada besi bekas, Mas. Lihat dulu, timbang dulu.\"";
            panel.gameObject.SetActive(true); input.IsBlocked = true; Refresh();
        }

        private void Close() { panel.gameObject.SetActive(false); input.IsBlocked = false; }

        private void Haggle()
        {
            var reply = game.Haggle(id, offer);
            offer = game.Seller(id).AskPerKg; Refresh(); voice.text = reply.Line;
        }

        private void Pay()
        {
            double paid = game.BuyAtDoor(id);
            Close(); hud.NoticeText.text = "Dibayar " + Rp(paid) + ". Sortir muatan di lapak Anda."; hud.RefreshNow();
        }

        private void Refresh()
        {
            var s = game.Seller(id);
            title.text = s.Title.ToUpperInvariant() + "  ·  " + s.Place;
            goods.text = "Barang: " + s.Grade + "  ·  " + (s.Weighed ? "ditimbang " : "katanya kira-kira ") + s.StockKg.ToString("0.#") + " kg";
            price.text = "Minta " + Rp(s.AskPerKg) + "/kg" + (s.Agreed ? " (sepakat)" : "") + "  ·  Tawaran Anda " + Rp(offer) + "/kg";
            bill.text = s.TakenKg <= 0 ? "Muatan penuh. Kas " + Rp(s.Cash) :
                "Muat " + s.TakenKg.ToString("0.#") + " kg, dibayar " + s.ChargedKg.ToString("0.#") + " kg\nTotal " + Rp(s.Total) + "  ·  Kas " + Rp(s.Cash);
        }
    }
}
