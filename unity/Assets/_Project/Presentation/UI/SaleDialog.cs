using UnityEngine;
using UnityEngine.UI;

namespace Steel.Presentation.UI
{
    public static class SaleDialog
    {
        public static void Build(CityHud hud)
        {
            var panel = UiFactory.Panel("Konfirmasi Jual", hud.SafeRoot, new Vector2(.5f, 0),
                new Vector2(580, 300), new Vector2(0, 205));
            hud.SalePanel = panel.gameObject;
            hud.SaleText = UiFactory.Text(panel, "", 21, new Vector2(520, 190), new Vector2(0, 42));
            UiFactory.Button(panel, "KONFIRMASI JUAL", new Vector2(265, 55), new Vector2(120, -104), hud.Confirm);
            UiFactory.Button(panel, "BATAL", new Vector2(160, 55), new Vector2(-175, -104), hud.Cancel);
            panel.gameObject.SetActive(false);
        }
    }
}
