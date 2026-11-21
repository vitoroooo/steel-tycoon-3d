using System;
using NUnit.Framework;
using Steel.Core.Scrap;
using Steel.Services;

namespace Steel.Tests.Services
{
    public sealed class LocalGameTests
    {
        [Test]
        public void ResourcesComposeClockPricesAndScrapWithoutScene()
        {
            IGameService game = new LocalGameService(new ResourceConfigSource());
            var original = game.Prices;
            var pile = game.CreatePile(100, ScrapGrade.Hms1);
            double raw = game.Quote(pile);
            pile.Inspect();
            pile.Sort();
            Assert.Greater(game.Quote(pile), raw);
            game.Advance(599);
            Assert.AreSame(original, game.Prices);
            game.Advance(1);
            Assert.AreEqual(600, game.Prices.RefreshedAtRealSeconds);
            Assert.AreEqual(28800, game.CatchUp(86400));
            Assert.AreEqual(49, game.GameHours);
            Assert.AreEqual(29400, game.Prices.RefreshedAtRealSeconds);
        }

        [Test]
        public void MissingResourcesAndNullSourceFailClearly()
        {
            var source = new ResourceConfigSource();
            Assert.Throws<InvalidOperationException>(() => source.Read("Missing"));
            Assert.Throws<ArgumentException>(() => source.Read(""));
            Assert.Throws<ArgumentNullException>(() => new LocalGameService(null));
        }
    }
}
