using NUnit.Framework;
using Steel.Core.Finance;

namespace Steel.Tests.Finance
{
    public sealed class FinanceDebtTests
    {
        [Test] public void ShortfallBecomesDebtAndInterestWaitsFor24RealHoursFromShortfall()
        {
            var s = new FinanceScenario(); var g = s.Game;
            g.Position = g.Rules.Home; g.BuyScanner(false, 500000);
            g.SettleFinance(4000, s.Business); g.ChargeOperatingExpense(100000);
            Assert.Zero(g.Cash); Assert.AreEqual(100000, g.CooperativeDebt);
            g.SettleFinance(4000 + 86400 - 1, s.Business);
            Assert.AreEqual(100000, g.CooperativeDebt);
            g.SettleFinance(4000 + 2 * 86400, s.Business);
            Assert.AreEqual(104040, g.CooperativeDebt);
            g.SettleFinance(4000 + 2 * 86400, s.Business);
            Assert.AreEqual(104040, g.CooperativeDebt);
            g.CreditProceeds(110000, 20000);
            Assert.AreEqual(5960, g.Cash); Assert.Zero(g.CooperativeDebt); Assert.Zero(g.InterestDue);
        }

        [Test] public void DebtAboveHalfAssetsTakesLeastUsedToolFirstAndCreditsSurplus()
        {
            var s = new FinanceScenario(20000000); var g = s.Game;
            s.BuyMotor(); g.Position = g.Rules.Home;
            g.BuyScanner(false, 500000);
            g.Position = g.World.Find("p0").Position; g.Magnetic("p0");
            g.ChargeOperatingExpense(g.Cash + 7000000);
            g.SettleFinance(FinanceRules.RealDay, s.Business);
            Assert.IsTrue(g.Magnet); Assert.IsFalse(g.Fleet.Find("triseda").Owned);
            Assert.AreEqual(4860000, g.Cash); Assert.Zero(g.CooperativeDebt);
        }

        [Test] public void InsolventBandarLosesOneToolPerDayThenGetsStarterCapitalCartAndEarnedLevel()
        {
            var s = new FinanceScenario(); var g = s.Game;
            g.Yard.bandar = g.Yard.applied = g.Yard.leased = true;
            g.CreditProceeds(50000, 50000); int level = g.BusinessLevel;
            g.ChargeOperatingExpense(g.Cash + 200000000);
            g.SettleFinance(FinanceRules.RealDay, s.Business);
            Assert.IsTrue(g.HasScale); Assert.IsFalse(g.HasLoader);
            Assert.AreEqual(2, g.FinanceCaste); Assert.Greater(g.CooperativeDebt, 0);
            g.SettleFinance(2 * FinanceRules.RealDay, s.Business);
            Assert.AreEqual(1, g.FinanceCaste); Assert.AreEqual(g.Rules.StartCash, g.Cash);
            Assert.Zero(g.CooperativeDebt); Assert.AreEqual(level, g.BusinessLevel);
            g.Position = g.Cart.Position; g.GrabCart(); Assert.IsTrue(g.Cart.Held);
        }

        [Test] public void DebtAtHalfOfAssetsDoesNotSeizeAfterInterestPosts()
        {
            var s = new FinanceScenario(); var g = s.Game;
            g.Position = g.Rules.Home; g.BuyScanner(false, 500000);
            g.ChargeOperatingExpense(250000d / 1.02);
            g.SettleFinance(FinanceRules.RealDay, s.Business);
            Assert.IsTrue(g.Magnet); Assert.AreEqual(250000, g.CooperativeDebt, .00001);
        }
    }
}
