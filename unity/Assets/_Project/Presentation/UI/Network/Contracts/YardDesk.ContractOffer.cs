using Steel.Services;

namespace Steel.Presentation.UI
{
    /// <summary>The form for a new contract offer: one key picks the line, one changes it, one sends.</summary>
    internal sealed partial class YardDesk
    {
        private static readonly int[] ContractKg = {5, 10, 25, 50, 100, 250, 500, 1000};
        private static readonly string[] ContractLines = {"pembeli", "berat", "lama", "harga", "batal"};
        private int contractLine, contractBuyer, contractWeight = 1, contractDays = NetworkContract.MinDays, contractRate = 100;

        private void DrawContractOffer(NetworkBoard board)
        {
            if (board.traders.Length == 0)
            {
                detail.text = "Belum ada pedagang lain di pasar ini.";
                Key(2,"Kembali",() => forms = false);
                return;
            }
            contractBuyer %= board.traders.Length;
            string buyer = board.traders[contractBuyer];
            int kg = ContractKg[contractWeight], price = board.reference * contractRate / 100;
            long total = (long)kg * price * contractDays;
            detail.text = "Tawarkan ke " + buyer + "   ·   HMS 1 " + kg + " kg/hari   ·   " + contractDays + " hari game" +
                "\n" + Rp(price) + "/kg (" + contractRate + "% acuan)   ·   Nilai kontrak " + Rp(total) +
                "\nPembeli mengunci dana per kiriman. Batal: penalti " + NetworkContract.PenaltyPercent + "% sisa kontrak.";
            Key(0,"Kirim tawaran",() =>
            {
                forms = false;
                NetworkRun(() => network.Offer(buyer,kg,price,contractDays));
            });
            Key(1,contractLine == 4 ? "Batal" : "Ubah " + ContractLines[contractLine],ChangeContractLine);
            Key(2,"Baris: " + ContractLines[contractLine],() => contractLine = (contractLine + 1) % ContractLines.Length);
        }

        private void ChangeContractLine()
        {
            if (contractLine == 0) contractBuyer++;
            else if (contractLine == 1) contractWeight = (contractWeight + 1) % ContractKg.Length;
            else if (contractLine == 2)
                contractDays = contractDays == NetworkContract.MaxDays ? NetworkContract.MinDays : contractDays + 1;
            else if (contractLine == 3) contractRate = contractRate >= 120 ? 80 : contractRate + 10;
            else { forms = false; contractLine = 0; }
        }
    }
}
