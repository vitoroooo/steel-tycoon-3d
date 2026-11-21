using UnityEngine;
using Steel.Services;

namespace Steel.Presentation.UI
{
    public static class VehicleHud
    {
        public static void Refresh(CityHud hud, VehicleView v)
        {
            hud.RouteText.text += v.Occupied ? "\nPIKAP · " + (Mathf.Abs(v.Speed) * 3.6f).ToString("N0") +
                " km/j · Sisa trip " + (v.RemainingTripSeconds / 60).ToString("N0") + " menit" :
                "\nPikap pinjaman · Muatan tersimpan " + v.CargoKg + " / " + v.CapacityKg + " kg";
        }
    }
}
