using Steel.Services;
using UnityEngine;

namespace Steel.Presentation.UI
{
    internal static class DartoDirections
    {
        internal static string Hint(IGameService game, GameView view)
        {
            if (view.GrantCollected || view.GrantReady)
            {
                var bike = game.VehicleOf(FleetCatalog.Triseda);
                if (bike.Owned) return "Seret kanan: kamera  ·  E / tombol kanan: interaksi";
                var offset = new Vector2(bike.X - view.PlayerX, bike.Z - view.PlayerZ);
                return "Pak Darto · Dealer " + offset.magnitude.ToString("N0") + " m: " +
                    (view.GrantReady ? "ambil hadiah setoran pertama." : "beli motor bak roda tiga.");
            }
            if (view.LoadSorted) return "Pak Darto · Jual muatan tersortir di jembatan timbang Pabrik AI.";
            if (view.CarriedKg > 0) return "Pak Darto · Bawa rongsok ke lapak dan sortir muatanmu.";
            return "Pak Darto · Periksa tumpukan di gang, lalu pungut sebelum ke lapak.";
        }
    }
}
