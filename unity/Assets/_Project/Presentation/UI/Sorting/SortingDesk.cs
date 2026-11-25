using System;
using Steel.Presentation.Input;
using Steel.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    internal sealed class SortingDesk
    {
        private readonly CityHud hud;
        private readonly IGameService game;
        private readonly SliceInput input;
        private readonly RectTransform panel;
        private readonly Text clue, title, hint;
        private string id;
        internal SortingDesk(CityHud hud, IGameService game, SliceInput input)
        {
            this.hud = hud; this.game = game; this.input = input;
            panel = UiFactory.Panel("Meja sortir", hud.SafeRoot, Vector2.one * .5f, new Vector2(786,430), Vector2.zero);
            title = Line(panel, "", 24, 716, 50, 0, 172);
            hint = Line(panel, "", 18, 716, 58, 0, 118);
            var card = UiFactory.Panel("Sampel", panel, Vector2.one * .5f, new Vector2(215,118), new Vector2(-245,0));
            card.GetComponent<Image>().color = new Color(.17f,.29f,.33f);
            clue = Line(card, "", 21, 194, 110, 0, 0);
            var drag = card.gameObject.AddComponent<SampleDrag>(); drag.Bins = new RectTransform[6];
            string[] names = { "HMS 1", "HMS 2", "Besi cor", "Campur", "Layak pakai", "Kotoran" };
            for (int i = 0; i < names.Length; i++)
            {
                var bin = UiFactory.Panel(names[i], panel, Vector2.one * .5f,
                    new Vector2(143,72), new Vector2(-53 + i % 3 * 151, 28 - i / 3 * 82));
                bin.GetComponent<Image>().color = new Color(.25f,.22f,.16f);
                Line(bin, names[i], 20, 140, 68, 0, 0); drag.Bins[i] = bin;
            }
            drag.Drop = bin => Submit(bin, false);
            Button("BATAL", -267, Close);
            Button("MAGNET", 14, () => Attempt(() => hint.text = game.InspectMagnet(id)));
            Button("PINDAI XRF", 254, () => Submit(-1, true));
            panel.gameObject.SetActive(false);
        }
        private void Button(string label, int x, UnityEngine.Events.UnityAction action) =>
            UiFactory.Button(panel, label, new Vector2(189,56), new Vector2(x,-155), action);
        private static Text Line(Transform root, string value, int size, int w, int h, int x, int y) =>
            UiFactory.Text(root, value, size, new Vector2(w,h), new Vector2(x,y), TextAnchor.MiddleCenter);
        internal void Open(string site)
        {
            id = site; clue.text = game.SampleClue(site); input.IsBlocked = true; panel.gameObject.SetActive(true);
            title.text = site == null ? "Meja sortir lapak" : "Periksa temuan";
            hint.text = "Seret sampel ke bak yang sesuai. Pelat tebal bersih masuk HMS 1; lembaran tipis HMS 2.";
        }
        private void Attempt(Action action)
        { try { action(); } catch (InvalidOperationException e) { hint.text = e.Message; } }
        private void Submit(int bin, bool scan) => Attempt(() =>
        {
            string result = game.Classify(id, bin, scan);
            if (id == null && !game.View.LoadSorted)
            { clue.text = game.SampleClue(null); hint.text = result + " Lanjutkan sampel berikut."; }
            else { Close(); hud.NoticeText.text = result + (id != null ? " Tekan Pungut." : " Siap dijual."); }
        });
        private void Close() { panel.gameObject.SetActive(false); input.IsBlocked = false; hud.RefreshNow(); }
    }
}
