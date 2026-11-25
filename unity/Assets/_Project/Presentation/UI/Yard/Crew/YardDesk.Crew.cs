using System;

namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        private bool crewCommunity;

        private void DrawCrew()
        {
            var c = game.CrewInfo();
            if (!c.Open)
            {
                detail.text = "Buka lapak Kasta 2 untuk mengatur Mandor, regu bongkar, dan hubungan warga.";
                return;
            }
            string operation = c.InvestigationLeft > 0 ? "Investigasi K3 • " + Math.Ceiling(c.InvestigationLeft) +
                " jam lagi" : c.BlockadeLeft > 0 ? "Blokade jalan • " + Math.Ceiling(c.BlockadeLeft) +
                " jam lagi" : c.Stop ?? "Lapak siap beroperasi";
            if (crewCommunity)
            {
                string residents = c.Protest ? "warga protes" : "warga tenang";
                detail.text = "Kesabaran warga " + c.Patience.ToString("0") + "/100 · " + residents +
                    " · " + operation + "\nKerja malam dan truk di jalan mengurangi kesabaran." +
                    "\nMandor " + (c.Senior ? "senior" : "reguler") + " · batas offline " + c.OfflineHours +
                    " jam nyata · insiden " + c.Incidents + " · denda terakhir " + Rp(c.LastFine);
                if (!c.Senior) Key(0, "Latih · " + Rp(c.Training), game.PromoteCrewForeman);
                if (c.Patience < 100 && c.CsrWait == 0) Key(1, "CSR · " + Rp(c.CsrCost), game.FundCrewCsr);
                if (c.CsrWait > 0) detail.text += "\nProgram CSR berikutnya dalam " + Math.Ceiling(c.CsrWait) + " jam.";
                Key(2, "Regu & shift", () => crewCommunity = false);
                return;
            }
            detail.text = "Mandor · " + c.Policy + " · shift " + c.Shift + " · " + operation +
                "\nStamina siang " + c.DayStamina.ToString("0") + " / malam " + c.NightStamina.ToString("0") +
                " · Moral " + c.Morale.ToString("0") + "/100\nGaji " + Rp(c.Wage) + "/hari game" +
                (c.Premium ? " (premi +10%)" : "") + " · " +
                (c.Policy == "Lembur" ? "Output +40%, moral turun." : c.Policy == "Hemat" ?
                    "Regu siang saja." : "Regu bergantian siang dan malam.");
            Key(0, "Kebijakan berikut", game.CycleCrewPolicy);
            Key(1, c.Premium ? "Gaji standar" : "Beri premi 10%", game.ToggleCrewPremium);
            Key(2, "Mandor & warga", () => crewCommunity = true);
        }
    }
}
