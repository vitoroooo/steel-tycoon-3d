using System;
using System.Linq;
using Steel.Core.Finance;
using Steel.Core.State.Saving;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        private double financeClockOffset;

        // The simulation clock excludes capped offline time. The financial calendar retains it.
        public void TickFinanceClock(double simulatedSeconds, double skippedSeconds, ConfigValues prices)
        {
            financeClockOffset += NumberRules.NonNegative(skippedSeconds, nameof(skippedSeconds));
            SettleFinance(simulatedSeconds + financeClockOffset, prices);
        }

        public FinanceSave SaveFinance() => accounts.Copy();

        public void LoadFinance(FinanceSave saved, double simulatedSeconds)
        {
            NumberRules.NonNegative(simulatedSeconds, nameof(simulatedSeconds));
            if (saved == null || saved.version == 0)
            {
                accounts = new FinanceSave { seconds = simulatedSeconds };
                financeClockOffset = 0;
                return;
            }
            if (saved.version != 1 || saved.level < 1 || saved.level > FinanceRules.MaxLevel ||
                saved.stockCost?.Length != 4 || saved.knownKg?.Length != 4 ||
                !Finite(saved.lifetimeProfit) || !Finite(saved.quarterProfit) || !Finite(saved.shipmentMargin) ||
                saved.seconds < simulatedSeconds || !Finite(saved.seconds) || saved.debt < 0 || !Finite(saved.debt) ||
                !Finite(saved.interestDue) || (saved.debt > 0 ? saved.interestDue <= saved.seconds : saved.interestDue != 0) ||
                !Finite(saved.lastTax) || saved.lastTax < 0 || saved.shipmentMargin > Yard.payout ||
                Yard.payout == 0 && saved.shipmentMargin != 0 || !Yard.bandar && (saved.scaleSeized || saved.loaderSeized))
                throw new FormatException("Buku keuangan tidak valid.");
            for (int i = 0; i < 4; i++)
            {
                NumberRules.NonNegative(saved.stockCost[i], "biaya stok");
                NumberRules.NonNegative(saved.knownKg[i], "berat stok");
                if (saved.knownKg[i] > Yard.stock[i] + .001) throw new FormatException("Berat stok tidak cocok.");
            }
            var activity = saved.activity ?? Array.Empty<AssetUse>();
            if (activity.Any(a => a == null || string.IsNullOrEmpty(a.id) || a.uses < 0) ||
                activity.Select(a => a.id).Distinct().Count() != activity.Length)
                throw new FormatException("Riwayat penggunaan tidak valid.");
            accounts = saved.Copy();
            financeClockOffset = FinanceSeconds - simulatedSeconds;
        }

        private static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
    }
}
