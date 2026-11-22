using Steel.Services;

namespace Steel.Presentation.UI
{
    public sealed partial class CityHud
    {
        private string ActionLabel(InteractionView near, GameView v)
        {
            if (near == null) return "CARI RONGSOK";
            switch (near.Id)
            {
                case "home": return v.CarriedKg == 0 ? "BUKA ARMADA" : v.LoadSorted ? "PAPAN HARGA" : "SORTIR";
                case "buyer": return "PAPAN HARGA";
                case "seller": return "TAWAR & BELI";
                case "factory": return "LIHAT HARGA";
                case "grant": return "AMBIL HADIAH";
                case "cart": return host.Game.Cart.Held ? "LEPAS GEROBAK" : "DORONG GEROBAK";
                case "vehicle": return VehicleText.Action(host.Game.Vehicle);
                default: return near.Inspected ? "PUNGUT" : "PERIKSA";
            }
        }
    }
}
