using System.Linq;
using UnityEngine;
using Steel.Services;
using Steel.Presentation.Vehicles;

namespace Steel.Presentation.UI
{
    /// <summary>The HUD's wording and context action for whichever vehicle the player is at or in.</summary>
    public static class VehicleHud
    {
        public static void Refresh(CityHud hud, VehicleView v)
        {
            if (v.Occupied)
                hud.RouteText.text += "\n" + v.Label.ToUpperInvariant() + " · " +
                    (Mathf.Abs(v.Speed) * 3.6f).ToString("N0") + " km/j · Sisa trip " +
                    (v.RemainingTripSeconds / 60).ToString("N0") + " menit";
            else if (v.Owned)
                hud.RouteText.text += "\n" + VehicleText.Name(v) + " · Muatan tersimpan " +
                    v.CargoKg + " / " + v.CapacityKg + " kg";
        }

        /// <summary>Buys, mounts or dismounts the vehicle in focus and returns what to tell the player.</summary>
        public static string Toggle(MotorDriver[] drivers, VehicleView focus)
        {
            drivers.First(d => d.VehicleId == focus.Id).Toggle();
            return focus.Occupied ? VehicleText.Name(focus) + " diparkir." :
                focus.Owned ? "Muatan sampai " + VehicleText.Load(focus.CapacityKg) + ". Tahan arah untuk melaju." :
                VehicleText.Name(focus) + " terbeli! Sekarang milik Anda.";
        }
    }
}
