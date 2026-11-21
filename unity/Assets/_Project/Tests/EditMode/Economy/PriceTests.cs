using System;
using NUnit.Framework;
using Steel.Core.Economy;

namespace Steel.Tests.Economy
{
    public sealed class PriceTests
    {
        [Test]
        public void FractionalElapsedBoundaryAndIntegerBucketsAgree()
        {
            var c = new EconomyConfig(TestConfig.With("Economy", "refreshRealSeconds", 600.1));
            var elapsed = new AiPriceBook(c);
            var ticked = new AiPriceBook(c);
            elapsed.Refresh(4200.7, c.SupplyTargetKg);
            ticked.RefreshBucket(7, c.SupplyTargetKg);
            Assert.AreEqual(7, elapsed.CurrentBucket);
            Assert.AreEqual(ticked.Current.Ibn, elapsed.Current.Ibn);
            Assert.AreEqual(7 * 600.1, elapsed.Current.RefreshedAtRealSeconds, 1e-8);
        }

        private EconomyConfig config;

        [SetUp]
        public void SetUp() => config = new EconomyConfig(
            TestConfig.With("Economy", "dailyNoiseFraction", 0));

        [Test]
        public void AiPricesUseFinalEconomyCoefficientsAndRefreshBoundary()
        {
            var prices = new AiPriceBook(config);
            Assert.IsNull(prices.Current);
            Assert.IsTrue(prices.Refresh(0, config.SupplyTargetKg));
            AiQuote first = prices.Current;
            Assert.AreEqual(5500, first.ScrapReference, 1e-8);
            Assert.AreEqual(3850, first.BuyFromK1, 1e-8);
            Assert.AreEqual(5500, first.BuyFromK2, 1e-8);
            Assert.AreEqual(3960, first.SellToK1, 1e-8);
            Assert.AreEqual(4675, first.SellToBandar, 1e-8);
            Assert.AreEqual(5830, first.SellToSmelter, 1e-8);
            Assert.AreEqual(10000, first.BuyIngot);
            Assert.IsFalse(prices.Refresh(599, 0));
            Assert.AreSame(first, prices.Current);
            Assert.IsTrue(prices.Refresh(600, 0));
            Assert.AreEqual(600, prices.Current.RefreshedAtRealSeconds);
            Assert.AreNotEqual(first.Ibn, prices.Current.Ibn);
        }

        [Test]
        public void LateRefreshAlignsToSameQuoteAsSteppedCalls()
        {
            var direct = new AiPriceBook(config);
            var stepped = new AiPriceBook(config);
            direct.Refresh(1250, config.SupplyTargetKg);
            for (int i = 0; i <= 2; i++) stepped.Refresh(i * 600, config.SupplyTargetKg);
            Assert.AreEqual(stepped.Current.Ibn, direct.Current.Ibn);
            Assert.AreEqual(1200, direct.Current.RefreshedAtRealSeconds);
            Assert.Throws<ArgumentOutOfRangeException>(() => direct.Refresh(1249, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => direct.Refresh(double.NaN, 0));
        }

        [Test]
        public void MarginAccountsForLossTaxAndOperatingCost()
        {
            Assert.AreEqual(-260, Margin.PerSaleKg(3960, 3850, 150));
            Assert.AreEqual(4700 * .98 - 3300 / .97 - 150,
                Margin.PerSaleKg(3300, 4700, 150, .03, config.TransactionTax), 1e-8);
            Assert.AreEqual(0, Margin.PerSaleKg(0, 0));
        }

        [TestCase(-.1)]
        [TestCase(1)]
        [TestCase(double.NaN)]
        public void InvalidLossCannotProduceMargin(double loss)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Margin.PerSaleKg(1, 2, 0, loss));
        }
    }
}
