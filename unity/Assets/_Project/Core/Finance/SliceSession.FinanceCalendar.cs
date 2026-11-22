using System;
using System.Linq;
using Steel.Core.Finance;
using Steel.Core.State.Saving;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        // Absolute elapsed real time since the career began. Passing the same target is idempotent.
        public void SettleFinance(double realSeconds, ConfigValues prices)
        {
            NumberRules.NonNegative(realSeconds, nameof(realSeconds));
            while (FinanceSeconds < realSeconds)
            {
                double quarterEnd = QuarterProfit == 0 && CooperativeDebt == 0 ? double.PositiveInfinity :
                    FinanceRules.NextQuarter(FinanceSeconds);
                double debtReview = CooperativeDebt > 0 ? InterestDue : double.PositiveInfinity;
                double due = Math.Min(quarterEnd, debtReview);
                if (due > realSeconds) { accounts.seconds = realSeconds; return; }
                accounts.seconds = due;
                if (due == debtReview) ReviewDebt(prices);
                if (due == quarterEnd) CloseQuarter();
            }
        }

        private void CloseQuarter()
        {
            double tax = FinanceRules.QuarterTax(QuarterProfit, FinanceCaste);
            accounts.quarterProfit = 0;
            if (tax == 0) return;
            CollectDue(tax);
            RecordLifetime(-tax);
            accounts.lastTax = tax;
        }

        private void ReviewDebt(ConfigValues prices)
        {
            double interest = CooperativeDebt * FinanceRules.DailyInterest;
            accounts.debt += interest;
            RecordOperatingProfit(-interest);
            accounts.interestDue = FinanceSeconds + FinanceRules.RealDay;
            var assets = Collateral(prices).OrderBy(a => Uses(a.Id)).ThenBy(a => a.Id, StringComparer.Ordinal).ToList();
            if (CooperativeDebt <= (assets.Sum(a => a.Value) + WorkingCapital()) / 2) return;
            if (assets.Count > 0)
            {
                var item = assets[0];
                item.Take();
                CreditCash(item.Value);
                accounts.activity = accounts.activity.Where(a => a.id != item.Id).ToArray();
                accounts.notice = "Koperasi menyita " + item.Label + " untuk mengangsur utang.";
            }
            if (CooperativeDebt > 0 && assets.Count <= 1)
            {
                bool wasMill = Mill.opened;
                LiquidateStock();
                if (CooperativeDebt > 0) RestartBusiness(prices,wasMill);
            }
        }

        private void RestartBusiness(ConfigValues prices,bool wasMill)
        {
            Mill = new MillSave(); Tender = new TenderSave(); LoadSites(null); RestoreCrew(null);
            Yard = new YardSave { bandar = wasMill, applied = wasMill, leased = wasMill,
                leaseUntil = wasMill ? (FinanceSeconds-financeClockOffset)/600+prices.Positive("leaseHours") : 0 };
            accounts.scaleSeized = accounts.loaderSeized = false;
            accounts.debt = accounts.interestDue = 0;
            accounts.quarterProfit = 0;
            Cash = wasMill ? prices.Positive("minimumCash")-prices.Positive("loader")-prices.Positive("scale")
                : Rules.StartCash;
            accounts.notice = "Usaha dimulai kembali di Kasta " + Caste + ". Modal awal cair, level dan gerobak tetap.";
        }
    }
}
