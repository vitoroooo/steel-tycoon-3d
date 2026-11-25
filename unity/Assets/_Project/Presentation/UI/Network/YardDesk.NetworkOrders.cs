using Steel.Services;

namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        private void DrawNetworkLots(NetworkBoard board)
        {
            if (board.lots.Length == 0)
            {
                detail.text = "Belum ada lot HMS 1 di papan pasar.";
                Key(0,"Muat ulang",() => NetworkRun(network.Refresh));
            }
            else
            {
                networkLot %= board.lots.Length;
                var lot = board.lots[networkLot];
                bool own = lot.seller == board.id;
                detail.text = "Lot " + (networkLot + 1) + "/" + board.lots.Length + "   ·   " + lot.name +
                    "\nHMS 1   ·   " + lot.kg + " kg × " + Rp(lot.price) + "/kg   ·   Total " + Rp(lot.Gross);
                if (own) Key(0,"Tarik lot saya",() => NetworkRun(() => network.Take(lot.id,true)));
                else if (lot.price < board.minimum || lot.price > board.maximum)
                    detail.text += "\nHarga di luar batas acuan terkini.";
                else if (board.cash < lot.Gross) detail.text += "\nKas akun pasar kurang.";
                else Key(0,"Bayar " + Rp(lot.Gross),() => NetworkRun(() => network.Take(lot.id,false)));
                if (board.lots.Length > 1)
                    Key(1,"Lot berikut",() => networkLot = (networkLot + 1) % board.lots.Length);
            }
            Key(2,"Kembali",() => NetworkOpen(NetworkTab.Account));
        }

        private void DrawNetworkSale(NetworkBoard board)
        {
            const int kg = 5;
            int price = board.reference * networkRate / 100;
            detail.text = "Titip 5 kg HMS 1   ·   " + Rp(price) + "/kg   ·   " + networkRate + "% acuan\n" +
                "Hasil kotor " + Rp(kg * price) + "   ·   Pajak penjual 2% saat terjual.";
            if (board.stock >= kg) Key(0,"Pasang lot 5 kg",() => NetworkRun(() => network.Sell(kg,price)));
            else detail.text += "\nStok pasar kurang dari 5 kg.";
            Key(1,"Ubah harga",() => networkRate = networkRate == 120 ? 80 : networkRate + 20);
            Key(2,"Kembali",() => NetworkOpen(NetworkTab.Account));
        }
    }
}
