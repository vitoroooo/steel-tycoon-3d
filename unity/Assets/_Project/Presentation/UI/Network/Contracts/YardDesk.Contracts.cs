using Steel.Services;

namespace Steel.Presentation.UI
{
    /// <summary>Buku usaha page for direct contracts with another trader (PRD C.2).</summary>
    internal sealed partial class YardDesk
    {
        private int contractShown;

        private void DrawContracts()
        {
            feedback.text = networkMessage;
            if (networkBusy) { detail.text = "Menunggu jawaban " + network.Host + "…"; return; }
            var board = network.Board;
            if (board == null) { detail.text = "Hubungkan akun di halaman Pasar jaringan untuk membuat kontrak langsung."; return; }
            if (network.HasPending)
            {
                detail.text = "Jawaban pasar belum diterima. Periksa hasilnya dengan ID pesanan yang sama.";
                Key(0,"Periksa pesanan",() => NetworkRun(network.Retry));
                return;
            }
            if (forms) { DrawContractOffer(board); return; }
            Key(2,"Tawaran baru",() => { forms = true; networkMessage = ""; });
            if (board.contracts.Length == 0)
            {
                detail.text = "Belum ada kontrak. Tawarkan kiriman HMS 1 harian ke pedagang lain selama " +
                    NetworkContract.MinDays + "–" + NetworkContract.MaxDays + " hari game.";
                Key(0,"Muat ulang",() => NetworkRun(network.Refresh));
                return;
            }
            contractShown %= board.contracts.Length;
            var c = board.contracts[contractShown];
            detail.text = "Kontrak " + (contractShown + 1) + "/" + board.contracts.Length + "   ·   " +
                (c.Selling ? "Kirim ke " : "Terima dari ") + c.Partner + "   ·   " + ContractStatus(c) +
                "\nHMS 1   ·   " + c.kg + " kg/hari × " + Rp(c.price) + "/kg   ·   " + Rp(c.Daily) + " per kiriman" +
                "\nTerkirim " + c.delivered + "/" + c.days + " hari" +
                (c.Active ? "   ·   Dana terkunci " + Rp(c.locked) + "   ·   Kiriman berikut jam kota " + c.due : "");
            if (c.Offered && !c.Selling)
            {
                if (board.cash >= c.Daily) Key(0,"Terima, kunci " + Rp(c.Daily),() => NetworkRun(() => network.Accept(c.id)));
                else detail.text += "\nKas pasar kurang untuk mengunci kiriman pertama.";
            }
            else if (c.Offered) Key(0,"Tarik tawaran",() => NetworkRun(() => network.End(c.id)));
            else if (c.Active) Key(0,"Batal, penalti " + Rp(c.Penalty),() => NetworkRun(() => network.End(c.id)));
            else Key(0,"Muat ulang",() => NetworkRun(network.Refresh));
            if (board.contracts.Length > 1)
                Key(1,"Kontrak berikut",() => contractShown = (contractShown + 1) % board.contracts.Length);
        }

        private static string ContractStatus(NetworkContract c) =>
            c.Offered ? "menunggu jawaban" : c.Active ? "berjalan" : c.status == "done" ? "selesai" :
            c.status == "withdrawn" ? "ditarik" : "batal, penalti " + Rp(c.penalty);
    }
}
