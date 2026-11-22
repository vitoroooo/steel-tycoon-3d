using Steel.Services;
using UnityEngine;

namespace Steel.Presentation.UI
{
    /// <summary>The one-line "what next" hint at the bottom of the HUD.</summary>
    internal static class DartoDirections
    {
        private static readonly string[] Compass = {"U","TL","T","TG","S","BD","B","BL"};
        internal static string To(GameView v, float x, float z)
        {
            var d = new Vector2(x-v.PlayerX,z-v.PlayerZ);
            return Mathf.CeilToInt(d.magnitude) + " m " + Compass[(Mathf.RoundToInt(Mathf.Atan2(d.x,d.y)*4/Mathf.PI)+8)%8];
        }
        internal static string Hint(IGameService game, GameView view, CityLayout map)
        {
            string errand = Errand(game, view, map);
            if (errand != null) return errand;
            if (view.LoadSorted && game.HasSteel) return "Pabrik " + To(view,map.Factory.X,map.Factory.Z) +
                " · Jual langsung, atau bandingkan harga bandar di lapak.";
            if (view.CarriedKg > 0) return "Lapak " + To(view,map.Home.X,map.Home.Z) +
                (view.LoadSorted ? " · Buka papan harga, pilih Toko Bangunan untuk barang layak pakai." : " · Sortir muatan di meja.");
            return IncomeHint.Text(game.Income, view, map);
        }
        /// <summary>One-off steps after the first sale; null once only everyday earning is left to do.</summary>
        private static string Errand(IGameService game, GameView view, CityLayout map)
        {
            if (!view.GrantCollected && !view.GrantReady) return null;
            var bike = game.VehicleOf(FleetCatalog.Triseda);
            if (bike.Owned && game.MotorRoute)
            {
                if (!game.ReadWeather) return "Ketuk Buku usaha → Cuaca untuk melihat prakiraan.";
                return game.StoredUnderRoof ? null : "Saat membawa stok ke lapak, pilih Cuaca → Titip di teras.";
            }
            if (bike.Owned) return "Lapak " + To(view,map.Home.X,map.Home.Z) + " · Parkirkan motor kosong dan pesan rute.";
            if (view.GrantReady) return "Dealer " + To(view,bike.X,bike.Z) + " · ambil hadiah setoran pertama.";
            return view.Cash < bike.Price ? null : "Dealer " + To(view,bike.X,bike.Z) + " · beli motor bak roda tiga.";
        }
    }
}
