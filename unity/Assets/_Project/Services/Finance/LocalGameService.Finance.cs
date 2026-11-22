using Steel.Core.Finance;

namespace Steel.Services
{
    public struct FinanceView
    {
        public int Level;
        public double Lifetime, Profit, TaxEstimate, Rate, LastTax, Assets, Debt, DaysToTax, DaysToInterest;
        public double ScalePrice, LoaderPrice;
        public bool ReplaceScale, ReplaceLoader;
        public string Notice;
    }

    public partial interface IGameService
    {
        FinanceView Finances();
        void ReplaceYardTool(bool loader);
    }

    public sealed partial class LocalGameService
    {
        private void FinanceTime(double skipped = 0) => session.TickFinanceClock(clock.RealSeconds, skipped, business);

        public FinanceView Finances()
        {
            double seconds = session.FinanceSeconds;
            return new FinanceView
            {
                Level = session.BusinessLevel, Lifetime = session.LifetimeProfit, Profit = session.QuarterProfit,
                Rate = 100 * FinanceRules.TaxRate(session.FinanceCaste),
                TaxEstimate = FinanceRules.QuarterTax(session.QuarterProfit, session.FinanceCaste),
                LastTax = session.LastTax, Assets = session.BusinessAssetValue(business),
                Debt = session.CooperativeDebt,
                DaysToTax = (FinanceRules.NextQuarter(seconds) - seconds) / FinanceRules.RealDay,
                DaysToInterest = session.CooperativeDebt > 0 ? (session.InterestDue - seconds) / FinanceRules.RealDay : 0,
                ReplaceScale = session.Yard.bandar && !session.HasScale,
                ReplaceLoader = session.Yard.bandar && !session.HasLoader,
                ScalePrice = business.Positive("scale"), LoaderPrice = business.Positive("loader"),
                Notice = session.FinanceNotice
            };
        }

        public void ReplaceYardTool(bool loader) => session.ReplaceYardTool(loader, business);
    }
}
