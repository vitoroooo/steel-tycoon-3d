using System;
using System.Globalization;
using Steel.Presentation.Input;
using Steel.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        private readonly IGameService game;
        private readonly SliceInput input;
        private readonly RectTransform panel;
        private readonly Text title, detail, feedback;
        private readonly Button[] keys = new Button[3];
        private readonly Action[] actions = new Action[3];
        private int page, grade;
        private bool forms;
        private static string Rp(double n) => "Rp" + n.ToString("N0", CultureInfo.GetCultureInfo("id-ID"));
        internal YardDesk(CityHud hud, IGameService game, SliceInput input)
        {
            this.game = game; this.input = input;
            panel = UiFactory.Panel("Buku usaha", hud.SafeRoot, Vector2.one * .5f, new Vector2(812,430), Vector2.zero);
            title = Line(24, 170, 50); detail = Line(20, 76, 122); feedback = Line(17, -83, 50);
            for (int i = 0; i < keys.Length; i++)
            {
                int k = i;
                keys[i] = UiFactory.Button(panel,"Pilihan " + i, new Vector2(244,62), new Vector2((i-1)*259,-16),
                    () => Try(k));
            }
            UiFactory.Button(panel, "TUTUP", new Vector2(156,54), new Vector2(-285,-156), Close);
            UiFactory.Button(panel, "SEBELUMNYA", new Vector2(194,54), new Vector2(24,-156), () => Turn(-1));
            UiFactory.Button(panel, "BERIKUTNYA", new Vector2(194,54), new Vector2(271,-156), () => Turn(1));
            var tab = hud.SafeRoot.Find("Usaha"); tab.GetComponent<Image>().raycastTarget = true;
            tab.gameObject.AddComponent<OpenLedger>().Open = () =>
            {
                if (input.IsBlocked) return;
                panel.gameObject.SetActive(true); input.IsBlocked = true; Turn(0);
            };
            panel.gameObject.SetActive(false);
        }
        private Text Line(int size,int y,int height) =>
            UiFactory.Text(panel,"",size,new Vector2(738,height),new Vector2(0,y));
        private void Close() { panel.gameObject.SetActive(false); input.IsBlocked = false; }
        private void Turn(int n) { page = (page + n + 3) % 3; forms = false; feedback.text = ""; Draw(); }
        private void Try(int i)
        {
            try { actions[i]?.Invoke(); }
            catch (InvalidOperationException e) { feedback.text = e.Message; }
            Draw();
        }
        private void Key(int i, string label, Action action)
        {
            actions[i] = action; keys[i].gameObject.SetActive(label != null);
            if (label != null) keys[i].GetComponentInChildren<Text>().text = label;
        }
        private void Draw()
        {
            var y = game.YardInfo(grade);
            title.text = "Buku usaha   /   " + new[] {"Izin & lahan", "Alat sortir", "Papan setoran"}[page];
            for (int i = 0; i < 3; i++) Key(i,null,null);
            if (page == 0) Permit(y); else if (page == 1) Tools(y); else Orders(y);
        }
    }
}
