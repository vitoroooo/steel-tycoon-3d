using Steel.Services;

namespace Steel.Presentation.UI
{
    /// <summary>"Temuan barang reuse" on the Kejadian page: sell the find to Toko Konstruksi or keep it for a special SPK.</summary>
    internal sealed partial class YardDesk
    {
        private void DrawReuse(ReuseView r)
        {
            detail.text = (r.Found
                ? "Temuan reuse saat sortir: " + r.Kg.ToString("N0") + " kg. Toko Konstruksi membayar " + Rp(r.Price) +
                  " sekarang (" + r.HoursLeft.ToString("0") + " jam game lagi dijual otomatis)."
                : "Tidak ada temuan baru.") +
                "\nSimpanan untuk SPK khusus: " + r.StoreKg.ToString("N0") + " kg (Toko Konstruksi: " + Rp(r.StorePrice) + ")." +
                "\n" + (r.Order ? "SPK khusus: " + r.OrderKg.ToString("N0") + " kg dibayar " + Rp(r.OrderPay) + ", sisa " +
                    r.OrderHoursLeft.ToString("0") + " jam game." : "SPK khusus belum datang; menyimpan berarti menunggu harga tertinggi.");
            if (r.Found)
            {
                Key(0,"Jual ke toko",() => feedback.text = game.AnswerReuse(true));
                Key(1,"Simpan",() => feedback.text = game.AnswerReuse(false));
            }
            else if (r.Order) Key(0,"Kirim SPK khusus",() => feedback.text = "Dibayar " + Rp(game.DeliverReuseOrder()) + ".");
            else Key(1,"Jual simpanan",() => feedback.text = "Dibayar " + Rp(game.SellReuseStore()) + ".");
            Key(2,"Kembali",() => forms = false);
        }
    }
}
