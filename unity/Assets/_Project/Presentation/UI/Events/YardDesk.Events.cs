using Steel.Services;

namespace Steel.Presentation.UI
{
    /// <summary>Buku usaha page for the events of PRD appendix E: collectors' strike, price spike, dirty deposit, flash tender.</summary>
    internal sealed partial class YardDesk
    {
        private void DrawEvents()
        {
            EventView e = game.Events;
            string city = e.Strike ? "Pengepul AI mogok hari ini: setoran tinggal " + (e.Supply * 100).ToString("0") + "%."
                : e.Spike ? "Lonjakan harga: IBN naik " + (e.SpikeRise * 100).ToString("0.#") + "%. Kesempatan jual."
                : "Kota tenang hari ini.";
            string scale = !e.Dirty ? "Timbangan: tidak ada setoran bermasalah."
                : "Setoran berkotoran: " + e.DirtyGrade + " " + e.DirtyKg.ToString("N0") + " kg, kotoran " + (e.Dirt * 100).ToString("0") +
                  "%. Terima potongan " + Rp(e.DirtyPrice) + " atau tolak (" + e.DirtyHoursLeft.ToString("0") + " jam game).";
            string tender = !e.Flash ? "Tender kilat: belum ada."
                : "Tender kilat: " + (e.FlashKg / 1000).ToString("0.#") + " ton " + e.FlashProduct + " @ " + Rp(e.FlashPrice) + "/kg, terkirim " +
                  (e.FlashSent / 1000).ToString("0.#") + " ton, sisa " + e.FlashHoursLeft.ToString("0") + " jam game" +
                  (e.FlashTaken ? " (diambil)." : ".");
            detail.text = city + "\n" + scale + "\n" + tender + (string.IsNullOrEmpty(e.Note) ? "" : "\nTerakhir: " + e.Note);
            if (e.Dirty)
            {
                Key(0,"Terima potongan",() => feedback.text = game.AnswerDeposit(true));
                Key(1,"Tolak setoran",() => feedback.text = game.AnswerDeposit(false));
            }
            if (e.Flash && !e.FlashTaken) Key(2,"Ambil tender",() => feedback.text = game.TakeFlashTender());
            else if (e.Flash) Key(2,"Kirim " + e.FlashProduct,() => feedback.text = "Dibayar " + Rp(game.DeliverFlashTender()) + ".");
        }
    }
}
