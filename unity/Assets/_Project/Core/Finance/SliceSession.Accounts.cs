using System;
using Steel.Core.Finance;
using Steel.Core.State.Saving;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        private FinanceSave accounts = new FinanceSave();
        public int BusinessLevel => accounts.level;
        public double LifetimeProfit => accounts.lifetimeProfit;
        public double QuarterProfit => accounts.quarterProfit;
        public double LastTax => accounts.lastTax;
        public double CooperativeDebt => accounts.debt;
        public double InterestDue => accounts.interestDue;
        public double FinanceSeconds => accounts.seconds;
        public string FinanceNotice => accounts.notice;
        public int FinanceCaste => Yard.bandar ? 2 : 1;

        public void RecordOperatingProfit(double margin)
        {
            if (double.IsNaN(margin) || double.IsInfinity(margin))
                throw new ArgumentOutOfRangeException(nameof(margin));
            accounts.quarterProfit += margin;
            RecordLifetime(margin);
        }

        private void RecordLifetime(double amount)
        {
            accounts.lifetimeProfit += amount;
            while (BusinessLevel < FinanceRules.MaxLevel &&
                   LifetimeProfit >= FinanceRules.Threshold(BusinessLevel + 1)) accounts.level++;
        }

        public void CreditProceeds(double netCash, double realizedMargin)
        {
            NumberRules.NonNegative(netCash, nameof(netCash));
            RecordOperatingProfit(realizedMargin);
            CreditCash(netCash);
        }

        private void CreditCash(double amount)
        {
            double repaid = Math.Min(amount, CooperativeDebt);
            accounts.debt -= repaid;
            Cash += amount - repaid;
            if (CooperativeDebt == 0) accounts.interestDue = 0;
        }

        public void ChargeOperatingExpense(double amount)
        {
            NumberRules.NonNegative(amount, nameof(amount));
            RecordOperatingProfit(-amount);
            CollectDue(amount);
        }

        private void CollectDue(double amount)
        {
            double paid = Math.Min(Cash, amount);
            Cash -= paid;
            if (amount == paid) return;
            if (CooperativeDebt == 0) accounts.interestDue = FinanceSeconds + FinanceRules.RealDay;
            accounts.debt += amount - paid;
        }
    }
}
