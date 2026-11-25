using System.Globalization;

namespace Steel.Services
{
    /// <summary>Wording for a vehicle in prompts, so HUD and service describe it the same way.</summary>
    public static class VehicleText
    {
        private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("id-ID");

        public static string Name(VehicleView v) => v.Id == FleetCatalog.Triseda ? "Triseda (motor bak roda tiga)" : "Pikap bekas";

        public static string Load(double kilograms) =>
            kilograms >= 1000 ? (kilograms / 1000).ToString("0.#", Culture) + " ton" : kilograms.ToString("N0", Culture) + " kg";

        public static string Detail(VehicleView v) =>
            v.Occupied ? "Berhenti lalu turun" :
            v.Owned ? "Milik Anda · muatan " + v.CargoKg.ToString("N0", Culture) + " / " + Load(v.CapacityKg) :
            "Dijual Rp " + v.Price.ToString("N0", Culture) + " · kapasitas " + Load(v.CapacityKg);

        public static string Action(VehicleView v) =>
            (v.Occupied ? "TURUN " : v.Owned ? "NAIK " : "BELI ") + v.Label.ToUpperInvariant();
    }
}
