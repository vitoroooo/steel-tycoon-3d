using System;
using Steel.Services;

namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        private void Permit(YardView y)
        {
            if (forms)
            {
                detail.text = "Daftar PT perorangan · Rp15 juta\nDokumen mana yang membuktikan jenis usaha?\n" +
                    "Berkas lengkap: izin aktif dalam 24 jam game. Salah berkas: tambah 24 jam.";
                Key(0,"Kode usaha besi tua",() => File(true));
                Key(1,"STNK motor bak",() => File(false)); Key(2,"Batal",() => forms = false); return;
            }
            string permit = !y.PermitFiled ? "belum diajukan" : y.PermitHours > 0 ?
                "diproses " + Math.Ceiling(y.PermitHours) + " jam game" : "aktif";
            detail.text = "Kasta " + y.Caste + "   ·   Reputasi " + y.Rep + "/30   ·   Kas " + Rp(y.Cash) +
                "\nPT & izin: " + permit + "   ·   Lahan: " + (y.Leased ? "tersisa " + Math.Ceiling(y.LeaseHours) +
                " jam game" : "belum disewa") + "\nNaik kasta perlu Rp250 juta; timbangan + loader Rp175 juta.";
            if (!y.PermitFiled) Key(0,"Urus PT / izin",() => forms = true);
            if (!y.Leased) Key(1,"Sewa  ·  Rp20 juta",() => game.Lease());
            if (y.Caste == 1) Key(2,"Buka lapak Kasta 2",() => game.OpenYard());
        }
        private void File(bool correct) { game.ApplyPermit(correct); forms = false; }
        private void Tools(YardView y)
        {
            detail.text = "Magnet menandai bahan yang menempel, bukan grade pasti.\n" +
                "XRF memeriksa grade dan kotoran sekali sentuh (Kasta 2).\nBeli di lapak Anda.";
            for (int i = 0; i < 2; i++)
            {
                bool xrf = i == 1;
                if (game.OwnScanner(xrf)) detail.text += xrf ? "   ·   XRF dimiliki" : "   ·   Magnet dimiliki";
                else Key(i,(xrf ? "XRF" : "Magnet") + " · " + Rp(game.ScannerCost(xrf)),() => game.BuyScanner(xrf));
            }
        }
        private void Orders(YardView y)
        {
            detail.text = y.Grade + "   ·   Penawaran " + Rp(y.Bid) + "/kg   ·   Stok " + y.Stock + " kg\n" +
                "Harga pembanding " + Rp(y.Floor) + "/kg. Pengepul memilih harga yang bersaing.\n" +
                "Jual 15 ton: terima " + Rp(y.NetShipment) + " setelah ongkir.";
            Key(0,"Grade berikut",() => grade = (grade+1)%4);
            if (y.Caste < 2) { feedback.text = "Buka lapak Kasta 2 untuk menerima setoran."; return; }
            Key(1,y.Bid == 0 ? "Pasang harga beli" : "Ubah harga beli",() =>
            {
                double low = Math.Ceiling(y.Floor / 50)*50;
                game.SetBid(grade,y.Bid < low ? low : y.Bid < low + 250 ? low + 250 : 0);
            });
            Key(2,"Kirim 15 ton",() => feedback.text = "Diterima " + Rp(game.ShipGrade(grade)));
        }
    }
}
