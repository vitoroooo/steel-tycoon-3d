using System;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Slice
{
    public sealed class SliceSaleTests
    {
        private IGameService game;
        private CityLayout layout;
        [SetUp]
        public void Setup()
        {
            var source = new ResourceConfigSource(); game = new LocalGameService(source); layout = new CityLayout(source);
            var p = layout.Piles[0].Position; game.SetPosition(p.X, p.Z); game.Inspect("p0"); game.PickUp("p0");
        }
        private void Sort()
        { game.SetPosition(layout.Home.X, layout.Home.Z); game.SortAtHome(); }
        private void AtFactory() => game.SetPosition(layout.Factory.X, layout.Factory.Z);
        [Test]
        public void PreviewDoesNotPayAndConfirmedSalePaysExactlyOnce()
        {
            double initial = game.View.Cash; AtFactory();
            Assert.Throws<InvalidOperationException>(() => game.PreviewSale());
            Sort(); AtFactory(); var offer = game.PreviewSale();
            Assert.AreEqual(game.Prices.BuyFromK1 * 12, offer.Revenue, 1e-8);
            Assert.AreEqual(offer.Revenue, offer.Profit);
            Assert.AreEqual(initial, game.View.Cash);
            Assert.AreEqual(offer.Revenue, game.ConfirmSale(), 1e-8);
            Assert.AreEqual(initial + offer.Revenue, game.View.Cash, 1e-8);
            Assert.AreEqual(0, game.View.CarriedKg);
            Assert.Throws<InvalidOperationException>(() => game.ConfirmSale());
        }
        [Test]
        public void StalePriceRequiresNewPreview()
        {
            Sort(); AtFactory(); game.PreviewSale(); game.Advance(601);
            Assert.Throws<InvalidOperationException>(() => game.ConfirmSale());
            Assert.AreEqual(12, game.View.CarriedKg);
            var fresh = game.PreviewSale(); Assert.AreEqual(fresh.Revenue, game.ConfirmSale());
        }
        [Test]
        public void ConfirmationStillChecksFactoryDistance()
        {
            Sort(); AtFactory(); game.PreviewSale();
            game.SetPosition(100, 100);
            Assert.Throws<InvalidOperationException>(() => game.ConfirmSale());
            Assert.AreEqual(12, game.View.CarriedKg);
        }
    }
}
