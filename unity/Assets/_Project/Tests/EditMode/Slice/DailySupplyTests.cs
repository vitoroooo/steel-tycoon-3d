using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Slice
{
    public sealed class DailySupplyTests
    {
        [Test]
        public void OnlyEmptySitesRefillAtMidnightAndCarriedItemIsUntouched()
        {
            var source = new ResourceConfigSource();
            var g = new LocalGameService(source);
            var map = new CityLayout(source);
            var first = map.Piles[0].Position;
            var second = map.Piles[1].Position;
            g.SetPosition(first.X, first.Z);
            g.Inspect("p0"); g.PickUp("p0");
            g.SetPosition(second.X, second.Z); g.Inspect("p1");
            g.Advance(4 * 3600 - 1);
            Assert.IsTrue(g.IsCollected("p0"));
            g.Advance(1);
            Assert.IsFalse(g.IsCollected("p0"));
            Assert.IsTrue(g.Nearby.Inspected, "Untouched stock is not replaced");
            g.SetPosition(first.X, first.Z);
            Assert.IsFalse(g.Nearby.Inspected, "New stock needs its own inspection");
            g.SetPosition(map.Home.X, map.Home.Z); g.SortAtHome();
            g.SetPosition(map.Factory.X, map.Factory.Z);
            Assert.AreEqual(12, g.PreviewSale().Kilograms);
            g.ConfirmSale();
            g.CatchUp(8 * 3600);
            g.SetPosition(first.X, first.Z); g.Inspect("p0"); g.PickUp("p0");
            Assert.AreEqual(12, g.View.CarriedKg, "Offline days do not stack multiple supplies");
        }
    }
}
