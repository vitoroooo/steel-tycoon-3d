using System;
using NUnit.Framework;
using Steel.Core.Economy;

namespace Steel.Tests.Economy
{
    public sealed class IbnTests
    {
        [Test]
        public void FormulaMatchesBaselineAndSupplyClamps()
        {
            var c = new EconomyConfig(TestConfig.With("Economy", "dailyNoiseFraction", 0));
            var index = new IbnIndex(c);
            Assert.AreEqual(10000, index.Evaluate(0, c.SupplyTargetKg), 1e-8);
            Assert.AreEqual(11500, index.Evaluate(c.CycleRealSeconds / 4, c.SupplyTargetKg), 1e-8);
            Assert.AreEqual(10500, index.Evaluate(0, 0), 1e-8);
            Assert.AreEqual(9500, index.Evaluate(0, 100 * c.SupplyTargetKg), 1e-8);
        }

        [Test]
        public void NoiseIsSharedDeterministicAndConstantWithinRealDay()
        {
            var c = new EconomyConfig(TestConfig.With("Economy", "cycleAmplitude", 0));
            var index = new IbnIndex(c);
            Assert.AreEqual(index.Evaluate(0, c.SupplyTargetKg),
                index.Evaluate(86000, c.SupplyTargetKg));
            double next = c.BaseIbn * (1 + (new Random(c.NoiseSeed + 1).NextDouble() * 2 - 1)
                * c.DailyNoiseFraction);
            Assert.AreEqual(next, index.Evaluate(86400, c.SupplyTargetKg));
            Assert.AreEqual(next, new IbnIndex(c).Evaluate(86400, c.SupplyTargetKg));
        }

        [Test]
        public void InvalidInputsAreRejected()
        {
            var index = new IbnIndex(new EconomyConfig(TestConfig.Read("Economy")));
            Assert.Throws<ArgumentOutOfRangeException>(() => index.Evaluate(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => index.Evaluate(0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => index.Evaluate(double.NaN, 0));
        }
    }
}
