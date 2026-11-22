using Steel.Presentation.Input;
using Steel.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    internal sealed class DartoLetter
    {
        private readonly Text story;
        private readonly RectTransform envelope;
        private readonly IGameService game;
        internal DartoLetter(CityHud hud, IGameService game, SliceInput input)
        {
            this.game = game;
            envelope = UiFactory.Panel("Surat Pak Darto",hud.SafeRoot,new Vector2(.5f,1),
                new Vector2(456,110),new Vector2(0,-68));
            envelope.SetSiblingIndex(hud.SalePanel.transform.GetSiblingIndex());
            story = UiFactory.Text(envelope,"",19,new Vector2(422,104),Vector2.zero);
            envelope.gameObject.AddComponent<OpenLedger>().Open = () =>
            {
                if (input.IsBlocked) return;
                game.ReadStory(); Refresh();
            };
            Refresh();
        }
        private void Refresh()
        {
            envelope.gameObject.SetActive(game.Story < 2);
            story.text = game.Story == 0 ? "Pak Darto  ·  1/2\nGerobak ini buat kamu. Ada Rp500 ribu untuk memulai lapak.  ›" :
                "Pak Darto  ·  2/2\nCari pelat bekas di gang. Periksa, pungut, lalu bawa pulang untuk disortir.  ›";
        }
    }
}
