using System;
using System.Linq;
using NUnit.Framework;
using Steel.Core.State;
using Steel.Core.Time;
using Steel.Core.Economy;
using Steel.Core.Scrap;

namespace Steel.Tests.State
{
    public sealed class ConfigTests
    {
        [Test]
        public void FlatNumericJsonParsesInvariantNumbersAndEmptyObjects()
        {
            var values = NumericJson.Parse(" { \"a\": -1.5e2, \"b\": 0.25 } ");
            Assert.AreEqual(-150, values["a"]);
            Assert.AreEqual(.25, values["b"]);
            Assert.IsEmpty(NumericJson.Parse("{}"));
        }

        [TestCase("")]
        [TestCase("[]")]
        [TestCase("{\"a\":1,}")]
        [TestCase("{\"a\":1,\"a\":2}")]
        [TestCase("{\"a\":01}")]
        [TestCase("{\"a\":NaN}")]
        [TestCase("{\"a\":1e9999}")]
        [TestCase("{\"a\":\"text\"}")]
        [TestCase("{\"a\":1} trailing")]
        public void MalformedOrUnsupportedJsonFailsExplicitly(string json)
        {
            Assert.Throws<FormatException>(() => NumericJson.Parse(json));
        }

        [Test]
        public void MissingOrInvalidBalanceValuesCannotLoad()
        {
            Assert.Throws<ArgumentNullException>(() => NumericJson.Parse(null));
            Assert.Throws<FormatException>(() => new TimeConfig("{}"));
            Assert.Throws<FormatException>(() => new TimeConfig(
                TestConfig.With("Time", "offlineCapRealSeconds", 0)));
            Assert.Throws<FormatException>(() => new EconomyConfig(
                TestConfig.With("Economy", "dailyNoiseFraction", 1)));
            Assert.Throws<FormatException>(() => new ScrapConfig(
                TestConfig.With("Scrap", "reuseMax", 1)));
        }

        [Test]
        public void CoreAssemblyDoesNotReferenceUnity()
        {
            Assert.IsFalse(typeof(GameClock).Assembly.GetReferencedAssemblies()
                .Any(reference => reference.Name.StartsWith("Unity", StringComparison.Ordinal)));
        }
    }
}
