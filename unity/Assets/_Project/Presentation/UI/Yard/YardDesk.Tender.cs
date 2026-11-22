using System;

namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        private int priceStep;
        private void DrawTender()
        {
            var t = game.GovernmentOrder(priceStep);
            detail.text = "Jembatan Kali Cempaka   ·   300 ton H-beam\n" +
                (t.Awarded ? t.Delivered.ToString("N0") + " kg terkirim. Sisa " + Math.Ceiling(t.Hours) + " jam game." :
                (t.Open ? "Penawaran ditutup" : "Jadwal berikut") + " dalam " + Math.Ceiling(t.Hours) + " jam game.") +
                "\n" + (t.Notice ?? "Harga 60% · reputasi 25% · sertifikasi K3 15%.");
            if (!t.SafetyFiled && !t.Awarded)
            {
                detail.text += "\nAjukan sertifikat K3 di pabrik. Proses: 2 hari game.";
                Key(0,"K3 · Rp200 jt",game.ApplySafetyCertificate);
            }
            else if (t.SafetyWait > 0)
                detail.text += "\nSertifikat K3 terbit dalam " + Math.Ceiling(t.SafetyWait) + " jam game.";
            else if (t.Open)
            {
                Key(0,Rp(t.UnitPrice) + "/kg",() => priceStep = (priceStep+1)%3);
                Key(1,"Segel penawaran",() => game.OfferBridgeSteel(priceStep));
            }
            if (t.Awarded) Key(2,"Kirim H-beam",() => feedback.text = "Pembayaran " + Rp(game.DeliverBridgeSteel()));
        }
    }
}
