using System.Linq;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Street
{
    /// <summary>Buying at a door through the game service: look, weigh, pay, carry off, and none of it comes back by reloading.</summary>
    public sealed class DoorPurchaseTests
    {
        private ResourceConfigSource source;
        private LocalGameService g;

        [SetUp] public void Start() { source = new ResourceConfigSource(); g = new LocalGameService(source); }

        /// <summary>Stands at the first door whose guessed weight runs over the true one.</summary>
        private SellerView Padded()
        {
            for (int i = 0; i < g.SellerCount; i++)
            {
                var door = g.Seller("s" + i);
                g.SetPosition(door.X, door.Z);
                double guess = door.StockKg; g.WeighSeller(door.Id);
                if (g.Seller(door.Id).StockKg < guess) { Start(); g.SetPosition(door.X, door.Z); return g.Seller(door.Id); }
            }
            Assert.Fail("No door in the city pads its weight this round"); return null;
        }

        [Test]
        public void TheCityHasDoorsOfEveryKindAndStandingAtOneOffersIt()
        {
            Assert.GreaterOrEqual(g.SellerCount, 20);
            var kinds = Enumerable.Range(0, g.SellerCount).Select(i => g.Seller("s" + i).Kind).Distinct().ToArray();
            Assert.AreEqual(4, kinds.Length, "Houses, workshops, projects and markets all sell");
            var door = g.Seller("s0"); g.SetPosition(door.X, door.Z);
            Assert.AreEqual("seller", g.Nearby.Id); StringAssert.Contains(door.Title, g.Nearby.Title);
        }

        [Test]
        public void WeighingChargesTheTrueWeightInsteadOfTheGuess()
        {
            var before = Padded();
            g.WeighSeller(before.Id);
            var after = g.Seller(before.Id);
            Assert.IsTrue(after.Weighed); Assert.Less(after.StockKg, before.StockKg);
            Assert.Less(after.Total, before.Total, "The same goods cost less once weighed");
            Assert.AreEqual(after.TakenKg, after.ChargedKg, 1e-9);
        }

        [Test]
        public void PayingMovesCashIntoGoodsOnceAndTheGoodsMustBeLookedAtFirst()
        {
            var door = Padded();
            Assert.Throws<System.InvalidOperationException>(() => g.BuyAtDoor(door.Id));
            Assert.AreEqual(500000, g.View.Cash); Assert.Zero(g.View.CarriedKg);
            g.InspectSeller(door.Id); g.WeighSeller(door.Id);
            var bill = g.Seller(door.Id);
            Assert.AreEqual(System.Math.Min(bill.StockKg, 15), bill.TakenKg, "The hand sack takes 15 kg at most");
            double paid = g.BuyAtDoor(door.Id);
            Assert.AreEqual(bill.Total, paid); Assert.AreEqual(500000 - paid, g.View.Cash);
            Assert.AreEqual(bill.TakenKg, g.View.CarriedKg);
            Assert.AreEqual(bill.StockKg - bill.TakenKg, g.Seller(door.Id).StockKg, 1e-9, "What did not fit stays for sale");
        }

        [Test]
        public void ReloadingNeitherRestocksADoorNorResetsTheTalk()
        {
            var door = Padded();
            g.InspectSeller(door.Id); g.WeighSeller(door.Id);
            Assert.IsTrue(g.Haggle(door.Id, door.AskPerKg * .95).Deal, "Five percent off is above every seller's floor");
            Assert.Less(g.Seller(door.Id).AskPerKg, door.AskPerKg);
            g.BuyAtDoor(door.Id);
            var left = g.Seller(door.Id); double cash = g.View.Cash;
            g = LocalGameService.Resume(source, g.Save(5000), 5000, out _);
            var back = g.Seller(door.Id);
            Assert.AreEqual(cash, g.View.Cash); Assert.AreEqual(left.StockKg, back.StockKg, 1e-9);
            Assert.AreEqual(left.Stocked, back.Stocked); Assert.AreEqual(left.Weighed, back.Weighed);
            Assert.AreEqual(left.AskPerKg, back.AskPerKg); Assert.AreEqual(left.Agreed, back.Agreed);
            g.Advance(2 * 600);
            Assert.IsTrue(g.Seller(door.Id).Stocked, "A new round brings new goods");
            Assert.IsFalse(g.Seller(door.Id).Weighed);
        }
    }
}
