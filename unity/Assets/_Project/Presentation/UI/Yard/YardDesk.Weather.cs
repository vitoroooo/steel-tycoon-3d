using System;

namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        private void Forecast(bool land)
        {
            var w = game.Weather(land ? 0 : district);
            if (!land)
            {
                detail.text = w.Area + "   ·   " + (w.Rain ? "Hujan" : "Cerah") + "   ·   Tanah " + w.Soil +
                    "\n" + w.Forecast + "\nTeras lapak: " + w.CoveredKg + "/50 kg terlindung dari karat.";
                Key(0,"Kecamatan berikut",() => district = (district + 1) % 3);
                Key(1,"Titip di teras",() => game.Shelter(true)); Key(2,"Ambil dari teras",() => game.Shelter(false));
                return;
            }
            detail.text = "Lapak " + (w.Paved ? "beton" : w.Soil) + "   ·   Hanggar " +
                (w.Hangar ? "50 ton terpasang" : "belum ada") + "\n" +
                (w.Stuck ? "Trailer terjebak. Loader butuh 1 jam game; derek langsung." : w.Settlement > 0
                ? "Nota " + Rp(w.Settlement) + " · sisa " + Math.Max(0,w.DueHour-game.GameHours).ToString("0.0") + " jam game."
                : "Becek: bongkar +30%. Lumpur: trailer berisiko terjebak.");
            if (w.Stuck)
            { Key(0,"Tarik dengan loader",() => game.Rescue(false)); Key(1,"Derek · Rp2,5 juta",() => game.Rescue(true)); }
            else
            {
                if (!w.Hangar) Key(0,"Atap · Rp120 juta",() => game.ImproveLand(false));
                if (!w.Paved) Key(1,"Cor · Rp140 juta",() => game.ImproveLand(true));
            }
        }
    }
}
