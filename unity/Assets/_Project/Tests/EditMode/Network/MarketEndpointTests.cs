using System;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Network
{
    public sealed class MarketEndpointTests
    {
        [TestCase("http://pasar.invalid")]
        [TestCase("https://user@pasar.invalid")]
        [TestCase("https://pasar.invalid/pedagang")]
        [TestCase("https://pasar.invalid/#kunci")]
        [TestCase("https://pasar.invalid/?token=123")]
        [TestCase("https://127.0.0.1")]
        public void RejectsInsecureOrAmbiguousOrigin(string address) =>
            Assert.Throws<ArgumentException>(() => new NetworkMarket(address));

        [Test] public void TradingUsesVersionedApiOnChosenTlsOrigin()
        {
            var uri = MarketEndpoint.ForHost("https://pasar.invalid:9443");
            Assert.AreEqual("https://pasar.invalid:9443/v1/",uri.AbsoluteUri);
            Assert.AreEqual("pasar.invalid:9443",new NetworkMarket(uri.GetLeftPart(UriPartial.Authority)).Host);
        }
    }
}
