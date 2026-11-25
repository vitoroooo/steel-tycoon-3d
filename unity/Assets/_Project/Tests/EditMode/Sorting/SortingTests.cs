using System;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Sorting
{
    public sealed class SortingTests
    {
        [Test] public void GuessDoesNotRevealOnMissAndWasteIsExcludedFromTheWeighSlip()
        {
            var src = new ResourceConfigSource(); var g = new LocalGameService(src); var city = new CityLayout(src);
            g.SetPosition(city.Home.X,city.Home.Z); g.BuyScanner(false); Assert.Zero(g.View.Cash);
            Assert.Throws<InvalidOperationException>(() => g.BuyScanner(false));
            Assert.Throws<InvalidOperationException>(() => g.BuyScanner(true));
            g.SetPosition(g.Cart.X,g.Cart.Z); g.GrabCart();
            var p = city.Piles[1]; g.SetPosition(p.Position.X,p.Position.Z);
            StringAssert.Contains("Menempel",g.InspectMagnet(p.Id)); Assert.IsFalse(g.Nearby.Inspected);
            Assert.Throws<InvalidOperationException>(() => g.Classify(p.Id,3,false));
            Assert.Throws<InvalidOperationException>(() => g.PickUp(p.Id));
            g.Classify(p.Id,1,false); g.PickUp(p.Id); g.SetPosition(city.Home.X,city.Home.Z);
            Assert.Throws<InvalidOperationException>(() => g.Classify(null,0,true));
            g.Classify(null,1,false); Assert.IsTrue(g.View.LoadSorted);
            var resumed = LocalGameService.Resume(src,g.Save(100),100,out _);
            Assert.IsTrue(resumed.OwnScanner(false));
            Assert.AreEqual(18 * .92,resumed.InspectBid(0).Kg,.0001);
        }
    }
}
