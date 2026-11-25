using System;
using Steel.Services;

namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        private int product, feedGrade;
        private static readonly string[] Products = {"Ingot","H-beam","Baut industri"};
        private void FactoryDesk(bool equipment)
        {
            var m = game.MillInfo(product,feedGrade);
            if (!m.Open)
            {
                if (forms)
                {
                    detail.text = "Dokumen AMDAL\nPilih lampiran pengendalian dampak pabrik.";
                    Key(0,"Debu & kebisingan",() => { game.ApplyAmdal(true); forms = false; });
                    Key(1,"Nota penjualan",() => game.ApplyAmdal(false)); Key(2,"Batal",() => forms = false);
                    return;
                }
                detail.text = "Kantor pabrik · gerbang PT Baja Nusantara di tenggara kota\n" +
                    "Kasta 3: level 28, AMDAL aktif, reputasi 70, kas Rp3 miliar.\n" +
                    (m.Filed ? "AMDAL: " + (m.Ready ? "aktif" : Math.Ceiling(m.Wait) + " jam game lagi") :
                    "Pengajuan AMDAL: " + Rp(m.AmdalFee));
                if (!m.Filed) Key(0,"Ajukan AMDAL",() => forms = true);
                if (m.Ready) Key(1,"Bangun · Rp2,95 M",game.BuildMill);
                return;
            }
            if (equipment)
            {
                detail.text = "Peralatan pabrik · " + Products[product] + "\n" +
                    "Shredder menurunkan susut. Lini rolling dan baut membuka produk baru.\nHarga: " +
                    Rp(m.UpgradeCost);
                Key(0,"Ganti peralatan",NextProduct);
                if (product == 0 ? !m.Shredder : !m.HasLine)
                    Key(1,product == 0 ? "Pasang shredder" : "Pasang lini",() => game.InstallLine(product));
                if (!m.Rfid) Key(2,"RFID · Rp400 jt",() => game.InstallLine(3));
                return;
            }
            var y = game.YardInfo(feedGrade);
            detail.text = Products[product] + "   ·   Siap jual " + m.Goods.ToString("N0") + " kg   ·   " +
                Rp(m.Price) + "/kg\n" + y.Grade + " masuk: " + m.Input.ToString("N0") + " kg. Per muatan 10 ton.\n" +
                (m.Batch > 0 ? "Selesai dalam " + m.FinishIn.ToString("0.0") + " jam game." : "Tungku kosong.");
            Key(0,"Pilih produk",NextProduct);
            Key(1,"Pilih grade",() => feedGrade = (feedGrade+1)%4);
            if (m.Goods > 0) Key(2,"Jual ke pasar baja",() => feedback.text = "Diterima " + Rp(game.SellSteel(product)));
            else if (m.HasLine && m.Batch == 0) Key(2,"Mulai lebur",() => game.ChargeFurnace(product,feedGrade));
        }
        private void NextProduct() => product = (product+1)%Products.Length;
    }
}
