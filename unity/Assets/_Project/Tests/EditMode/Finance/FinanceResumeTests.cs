using System;
using NUnit.Framework;
using Steel.Core.Finance;

namespace Steel.Tests.Finance
{
    public sealed class FinanceResumeTests
    {
        [Test] public void UnpaidQuarterlyTaxBecomesDebtOnlyOnceAcrossResume()
        {
            var s = new FinanceScenario(0); var g = s.Game;
            s.Gather(); g.Position = g.Rules.Factory;
            var offer = g.Preview(s.Quote); g.Confirm(offer, s.Quote);
            g.Position = g.Rules.Home; g.BuyScanner(false, g.Cash);
            g.SettleFinance(FinanceRules.Quarter, s.Business);
            Assert.Zero(g.Cash); Assert.AreEqual(offer.Profit * .05, g.CooperativeDebt, .00001);
            var restored = new FinanceScenario().Game;
            restored.Load(g.Snapshot(3), 3, 0); restored.LoadFinance(g.SaveFinance(), FinanceRules.Quarter);
            restored.SettleFinance(FinanceRules.Quarter, s.Business);
            Assert.Zero(restored.QuarterProfit);
            Assert.AreEqual(g.CooperativeDebt, restored.CooperativeDebt);
            restored.SettleFinance(FinanceRules.Quarter + FinanceRules.RealDay, s.Business);
            Assert.AreEqual(g.CooperativeDebt * 1.02, restored.CooperativeDebt, .00001);
        }

        [Test] public void SaveRestoresOfflineTimeBeyondQueueCapWithoutChargingAgain()
        {
            var s = new FinanceScenario(); var g = s.Game;
            g.Position = g.Rules.Home; g.BuyScanner(false, 500000);
            g.ChargeOperatingExpense(100000);
            const int played = 28800, skipped = 2 * 86400 - played;
            g.TickFinanceClock(played, skipped, s.Business);
            var dto = g.SaveFinance();
            var resume = new FinanceScenario(); var restored = resume.Game;
            restored.Load(g.Snapshot(3), 3, 2);
            restored.LoadFinance(dto, played);
            restored.TickFinanceClock(played, 0, s.Business);
            Assert.AreEqual(104040, restored.CooperativeDebt);
            restored.TickFinanceClock(played + 86400 - 1, 0, s.Business);
            Assert.AreEqual(104040, restored.CooperativeDebt);
            restored.TickFinanceClock(played + 86400, 0, s.Business);
            Assert.AreEqual(106120.8, restored.CooperativeDebt, .0001);
            Assert.AreEqual(104040, dto.debt);
        }

        [Test] public void ShipmentProfitUsesProportionalCostAtSettlementAndSurvivesResume()
        {
            var s = new FinanceScenario(); var g = s.Game;
            g.Yard.bandar = g.Yard.leased = g.Yard.applied = true;
            const int grade = 1;
            g.Yard.stock[grade] = 20000;
            g.RecordStockPurchase(grade, 10000, 20000000);
            g.RecordStockPurchase(grade, 10000, 40000000);
            g.BookShipmentMargin(grade, 15000, 20000, 55000000);
            g.Yard.stock[grade] -= 15000;
            g.Yard.payout = 55000000; g.Yard.dispatchedKg = 15000; g.Yard.unloadHour = 10;
            Assert.Zero(g.QuarterProfit);
            var resumed = new FinanceScenario().Game;
            resumed.Load(g.Snapshot(3), 3, 0); resumed.LoadFinance(g.SaveFinance(), 0);
            resumed.PayShipment(9); Assert.Zero(resumed.QuarterProfit);
            resumed.PayShipment(10); resumed.PayShipment(10);
            Assert.AreEqual(10000000, resumed.QuarterProfit);
            Assert.AreEqual(15000000, resumed.SaveFinance().stockCost[grade]);
            resumed.SettleFinance(FinanceRules.Quarter, s.Business);
            Assert.AreEqual(1000000, resumed.LastTax);
        }

        [Test] public void LegacyStockAndPendingInvoiceHaveNoInventedProfit()
        {
            var s = new FinanceScenario(); var g = s.Game;
            g.Yard.stock[1] = 5000; g.Yard.payout = 3000; g.Yard.dispatchedKg = 10;
            g.LoadFinance(null, 600); g.PayShipment(11);
            Assert.AreEqual(0, g.QuarterProfit);
            g.RecordStockPurchase(1, 5000, 2000000); g.Yard.stock[1] += 5000;
            g.BookShipmentMargin(1, 10000, 10000, 6000000);
            g.Yard.payout = 6000000; g.Yard.dispatchedKg = 10000; g.Yard.stock[1] = 0;
            g.PayShipment(12);
            Assert.AreEqual(1000000, g.QuarterProfit);
        }

        [Test] public void CorruptFinanceClockIsRejected()
        {
            var g = new FinanceScenario().Game;
            var dto = g.SaveFinance(); dto.debt = 10000; dto.interestDue = dto.seconds;
            Assert.Throws<FormatException>(() => g.LoadFinance(dto, 0));
        }
    }
}
