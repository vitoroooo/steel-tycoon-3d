namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        private int outlet = 1;
        private void DrawSatellite()
        {
            var v = game.Satellite(outlet);
            detail.text = "Satelit " + v.Area + "   ·   HMS 2 " + v.Stock.ToString("N0") + " kg\n" +
                "Beli " + Rp(v.Bid) + "/kg · harga pesaing " + Rp(v.Floor) + "/kg\n" +
                (v.Leased ? "Lapak aktif. Kirim 15 ton ke pusat seharga Rp3 juta." :
                    "Peralatan dan sewa: " + Rp(v.Fee));
            Key(0,"Ganti kecamatan",() => outlet = outlet == 1 ? 2 : 1);
            if (!v.Leased) Key(1,"Bayar sewa",() => game.LeaseSatellite(outlet));
            else Key(1,v.Bid > 0 ? "Hentikan pembelian" : "Buka pembelian",() => game.PriceSatellite(outlet,v.Bid == 0));
            if (v.Stock >= 15000) Key(2,"Kirim 15 ton",() => game.HaulSatellite(outlet));
        }
    }
}
