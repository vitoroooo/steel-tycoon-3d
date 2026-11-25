using Steel.Services;

namespace Steel.Presentation.UI
{
    internal sealed partial class YardDesk
    {
        private bool inspectLoader;

        private void DrawMaintenance()
        {
            MaintenanceView m = game.MaintenanceInfo(inspectLoader);
            if (!m.Owned)
            {
                detail.text = "Timbangan dan loader tersedia setelah lapak Kasta 2 dibuka.";
                return;
            }
            string tool = inspectLoader ? "Loader" : "Timbangan";
            string donor = inspectLoader ? "timbangan" : "loader";
            string status = m.RepairRemaining > 0 ? "Perbaikan · sisa " + m.RepairRemaining.ToString("0.00") + " jam" :
                m.Condition <= 0 ? "Rusak" : "Siap";
            detail.text = tool + " · " + m.Condition.ToString("0.0") + "% · " + status +
                "\nJam operasi " + m.OperatingHours.ToString("0.00") + " · Breakdown " + m.Breakdowns +
                " · Kecelakaan K3 " + m.Accidents;
            if (m.Condition <= 0 && m.RepairRemaining == 0 && forms)
            {
                RepairChoices(m, tool, donor);
                return;
            }
            detail.text += "\nKeausan " + m.NormalWear + "%/jam atau " + m.HeavyWear +
                "%/jam jika beban di atas " + m.HeavyAbove.ToString("0%") + ".";
            if (m.RiskRemaining > 0)
                detail.text += "\nRisiko darurat " + m.BreakdownChance.ToString("0%") +
                    " per muatan berat · " + m.RiskRemaining.ToString("0.00") + " jam game.";
            Key(0, "Lihat " + donor, () => { inspectLoader = !inspectLoader; forms = false; });
            if (m.Condition <= 0 && m.RepairRemaining == 0)
                Key(1, "Pilihan perbaikan", () => forms = true);
            else Key(1, "Perbarui kondisi", () => feedback.text = "Diperiksa pada jam game " +
                game.GameHours.ToString("0.00") + ".");
        }

        private void RepairChoices(MaintenanceView m, string tool, string donor)
        {
            detail.text = tool + " rusak · Suku cadang asli " + Rp(m.OriginalPrice) + " · " + m.OriginalHours + " jam game.";
            Key(0, "Suku cadang asli", () => StartRepair(0));
            if (m.NeedsOriginal)
            {
                detail.text += "\nAlat ini sudah menjadi donor dan memerlukan suku cadang asli.";
                Key(1, "Kembali", () => forms = false);
                return;
            }
            detail.text += "\nDonor gratis · " + m.DonorHours + " jam. " + donor + " jadi rusak dan wajib komponen asli." +
                "\nDarurat " + Rp(m.EmergencyPrice) + " · " + m.EmergencyHours * 60 + " menit. Risiko rusak " +
                m.BreakdownChance.ToString("0%") + "\nper muatan berat selama " + m.RiskHours + " jam game.";
            Key(1, "Pakai donor " + donor, () => StartRepair(1));
            Key(2, "Perbaikan darurat", () => StartRepair(2));
        }

        private void StartRepair(int choice)
        {
            forms = false;
            game.RepairMachine(inspectLoader, choice);
            feedback.text = "Perbaikan " + (inspectLoader ? "loader" : "timbangan") + " dimulai.";
        }
    }
}
