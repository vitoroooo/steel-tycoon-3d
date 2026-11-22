using System;
using NUnit.Framework;
using Steel.Core.Finance;
using Steel.Core.Scrap;

namespace Steel.Tests.Finance
{
    public sealed class FinanceIncomeTests
    {
        [Test] public void RealFactorySaleLevelsUpButPreviewGrantAndCapitalPurchasesEarnNothing()
        {
            var s = new FinanceScenario(); var g = s.Game;
            s.Gather(); g.Position = g.Rules.Factory;
            var invoice = g.Preview(s.Quote);
            Assert.Zero(g.QuarterProfit);
            g.Confirm(invoice, s.Quote);
            Assert.Greater(g.BusinessLevel, 1);
            Assert.Throws<InvalidOperationException>(() => g.Confirm(invoice, s.Quote));
            g.Position = g.Fleet.Find("triseda").Position;
            g.ClaimDealerGrant("triseda"); g.BuyMotor("triseda");
            Assert.AreEqual(invoice.Profit, g.QuarterProfit);
            double cash = g.Cash;
            g.SettleFinance(FinanceRules.Quarter, s.Business);
            Assert.AreEqual(cash - invoice.Profit * .05, g.Cash, .00001);
            Assert.Zero(g.QuarterProfit);
            g.SettleFinance(3 * FinanceRules.Quarter, s.Business);
            Assert.AreEqual(cash - invoice.Profit * .05, g.Cash, .00001);
        }

        [Test] public void ShippedLocalSaleTaxesOnlyMarginAfterGoodsAndFreight()
        {
            var s = new FinanceScenario(); var g = s.Game;
            var pile = new ScrapPile(8, ScrapGrade.Hms1, acquisitionCost: 5000);
            pile.Inspect(); g.Bag.Add(pile);
            g.Position = g.Rules.Home; g.SortAtHome();
            var invoice = g.Bid(s.Buyer, s.Quote, 3);
            Assert.Greater(invoice.Freight, 0);
            g.Settle(invoice, s.Quote, 3);
            Assert.AreEqual(invoice.Gross - invoice.Freight - 5000, g.QuarterProfit, .00001);
            Assert.Throws<InvalidOperationException>(() => g.Settle(invoice, s.Quote, 3));
        }

        [TestCase(1, 5)] [TestCase(2, 10)] [TestCase(3, 15)]
        public void TaxRateFollowsCasteAndNeverTaxesALoss(int caste, double expected)
        {
            Assert.AreEqual(expected, FinanceRules.QuarterTax(100, caste));
            Assert.Zero(FinanceRules.QuarterTax(-100, caste));
        }

        [Test] public void InvestingInitialCapitalProducesNoTaxAndDoesNotShieldActualOperatingProfit()
        {
            var s = new FinanceScenario(20000000); var g = s.Game;
            s.BuyMotor(); g.Position = g.Rules.Home;
            g.BuyScanner(false, s.Business.Positive("magnet"));
            double invested = g.Cash;
            g.SettleFinance(FinanceRules.Quarter, s.Business);
            Assert.AreEqual(invested, g.Cash); Assert.AreEqual(1, g.BusinessLevel);
            g.CreditProceeds(15000, 13000); g.ChargeOperatingExpense(3000);
            g.SettleFinance(2 * FinanceRules.Quarter, s.Business);
            Assert.AreEqual(500, g.LastTax);
            Assert.AreEqual(9500, g.LifetimeProfit);
        }

        [Test] public void LevelPersistsAfterLossAndCapsAtFifty()
        {
            var g = new FinanceScenario().Game;
            g.CreditProceeds(1e9, 1e9);
            Assert.AreEqual(50, g.BusinessLevel);
            g.ChargeOperatingExpense(1e9);
            Assert.Zero(g.LifetimeProfit); Assert.AreEqual(50, g.BusinessLevel);
        }
    }
}
