using Steel.Services;
using UnityEngine;

namespace Steel.Presentation.UI
{
    internal static class DartoDirections
    {
        private static readonly string[] Compass = {"U","TL","T","TG","S","BD","B","BL"};
        private static string To(GameView v, float x, float z)
        {
            var d = new Vector2(x-v.PlayerX,z-v.PlayerZ);
            return Mathf.CeilToInt(d.magnitude) + " m " + Compass[(Mathf.RoundToInt(Mathf.Atan2(d.x,d.y)*4/Mathf.PI)+8)%8];
        }
        internal static string Hint(IGameService game, GameView view, CityLayout map)
        {
            if (view.GrantCollected || view.GrantReady)
            {
                var bike = game.VehicleOf(FleetCatalog.Triseda);
                if (bike.Owned && game.MotorRoute)
                {
                    if (!game.ReadWeather) return "Ketuk Buku usaha → Cuaca untuk melihat prakiraan.";
                    if (!game.StoredUnderRoof) return "Saat membawa stok ke lapak, pilih Cuaca → Titip di teras.";
                    double wait = game.NextBikeMinutes;
                    return wait > 0 ? "Motor bak kembali dalam " + wait + " menit nyata." : "Motor bak siap. Cek kiriman di lapak.";
                }
                if (bike.Owned) return "Lapak " + To(view,map.Home.X,map.Home.Z) + " · Parkirkan motor kosong dan pesan rute.";
                return "Dealer " + To(view,bike.X,bike.Z) + " · " +
                    (view.GrantReady ? "ambil hadiah setoran pertama." : "beli motor bak roda tiga.");
            }
            if (view.LoadSorted && game.HasSteel) return "Pabrik " + To(view,map.Factory.X,map.Factory.Z) +
                " · Jual langsung, atau bandingkan harga bandar di lapak.";
            if (view.CarriedKg > 0) return "Lapak " + To(view,map.Home.X,map.Home.Z) +
                (view.LoadSorted ? " · Buka papan harga, pilih Toko Bangunan untuk barang layak pakai." : " · Sortir muatan di meja.");
            if (map.Piles.Length > 0 && !game.IsCollected(map.Piles[0].Id))
                return "Gang " + To(view,map.Piles[0].Position.X,map.Piles[0].Position.Z) + " · Periksa pelat, lalu pungut.";
            return "Periksa tumpukan di gang atau ambil kiriman di lapak.";
        }
    }
}
