using UnityEngine;
using UnityEngine.UI;
using Steel.Services;

namespace Steel.Presentation.UI
{
    public static class VehicleHud
    {
        public static Button Build(CityHud hud)
        {
            var button = UiFactory.Button(hud.SafeRoot, "MASUK PIKAP", new Vector2(200, 52),
                Vector2.zero, hud.ToggleVehicle);
            var rect = button.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = new Vector2(1, 0);
            rect.anchoredPosition = new Vector2(-135, 205); return button;
        }
        public static void Refresh(CityHud hud, VehicleView v)
        {
            hud.VehicleButton.gameObject.SetActive(v.Nearby || v.Occupied);
            hud.VehicleButton.GetComponentInChildren<Text>().text = v.Occupied ? "KELUAR PIKAP" : "MASUK PIKAP";
            hud.RouteText.text += v.Occupied ? "\nPIKAP · " + (Mathf.Abs(v.Speed) * 3.6f).ToString("N0") +
                " km/j · Sisa trip " + (v.RemainingTripSeconds / 60).ToString("N0") + " menit" :
                "\nPikap pinjaman · Muatan tersimpan " + v.CargoKg + " / " + v.CapacityKg + " kg";
        }
    }
}
