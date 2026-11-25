using System;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Slice
{
    public sealed class SliceInventoryTests
    {
        private IGameService game;
        private CityLayout layout;
        [SetUp]
        public void Setup()
        {
            var source = new ResourceConfigSource(); game = new LocalGameService(source); layout = new CityLayout(source);
        }
        private void AtPile(int i)
        { var p = layout.Piles[i].Position; game.SetPosition(p.X, p.Z); }
        [Test]
        public void HiddenPileRequiresInspectionAndCannotBeCollectedTwice()
        {
            AtPile(0); Assert.IsFalse(game.Nearby.Inspected);
            Assert.Throws<InvalidOperationException>(() => game.PickUp("p0"));
            game.Inspect("p0"); Assert.IsTrue(game.Nearby.Inspected);
            StringAssert.Contains("HMS 1", game.Nearby.Detail); game.PickUp("p0");
            Assert.AreEqual(12, game.View.CarriedKg); Assert.IsTrue(game.IsCollected("p0"));
            Assert.Throws<InvalidOperationException>(() => game.PickUp("p0"));
            Assert.Throws<InvalidOperationException>(() => game.Inspect("p0"));
        }
        [Test]
        public void HandSackTakesWhatFitsAndLeavesTheRestAtTheSite()
        {
            AtPile(0); game.Inspect("p0"); game.PickUp("p0");
            AtPile(1); game.Inspect("p1"); game.PickUp("p1");   // 12 kg carried, 18 kg pile, 15 kg sack
            Assert.AreEqual(15, game.View.CarriedKg); Assert.IsFalse(game.IsCollected("p1"));
            StringAssert.StartsWith("15 kg", game.Nearby.Detail); Assert.IsTrue(game.Nearby.Inspected);
            var full = Assert.Throws<InvalidOperationException>(() => game.PickUp("p1"));
            Assert.AreEqual("Muatan penuh. Sortir dan jual dahulu.", full.Message);
            Assert.AreEqual(15, game.View.CarriedKg);
        }
        [Test]
        public void LocationsAreValidatedInsideService()
        {
            game.SetPosition(100, 100);
            Assert.Throws<InvalidOperationException>(() => game.Inspect("p0"));
            Assert.Throws<InvalidOperationException>(() => game.SortAtHome());
            Assert.Throws<InvalidOperationException>(() => game.PreviewSale());
            Assert.Throws<ArgumentOutOfRangeException>(() => game.SetPosition(float.NaN, 0));
        }
        [Test]
        public void EmptyInventoryCannotSortOrSell()
        {
            game.SetPosition(layout.Home.X, layout.Home.Z);
            Assert.Throws<InvalidOperationException>(() => game.SortAtHome());
            game.SetPosition(layout.Factory.X, layout.Factory.Z);
            Assert.Throws<InvalidOperationException>(() => game.PreviewSale());
            Assert.Throws<InvalidOperationException>(() => game.ConfirmSale());
        }
    }
}
