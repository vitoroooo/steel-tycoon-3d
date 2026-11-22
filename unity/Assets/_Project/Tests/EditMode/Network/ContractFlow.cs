using System.Threading.Tasks;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Network
{
    /// <summary>Two Unity clients agree a direct contract (PRD C.2) through the market server, then one ends it.</summary>
    internal static class ContractFlow
    {
        internal static async Task Run(NetworkMarket seller, NetworkMarket buyer)
        {
            await seller.Refresh();
            CollectionAssert.AreEqual(new[] { buyer.Board.name }, seller.Board.traders);
            int price = seller.Board.reference;
            StringAssert.Contains("3 sampai 10", await seller.Offer(buyer.Board.name, 4, price, 2));
            await seller.Offer(buyer.Board.name, 4, price, 5);
            Assert.IsTrue(seller.Board.contracts[0].Offered && seller.Board.contracts[0].Selling);

            await buyer.Refresh();
            long cash = buyer.Board.cash, earned = seller.Board.cash;
            var offer = buyer.Board.contracts[0];
            Assert.IsFalse(offer.Selling);
            Assert.AreEqual(seller.Board.name, offer.Partner);
            await buyer.Accept(offer.id);
            var running = buyer.Board.contracts[0];
            Assert.IsTrue(running.Active);
            Assert.AreEqual(4L * price, running.locked);
            Assert.AreEqual(cash - running.Daily, buyer.Board.cash);

            // the seller breaks it before the first delivery: the lock goes back and he pays 10% of all five days
            long penalty = 5 * running.Daily / 10;
            Assert.AreEqual(penalty, running.Penalty);
            await seller.Refresh();
            await seller.End(running.id);
            await buyer.Refresh();
            Assert.AreEqual("broken", buyer.Board.contracts[0].status);
            Assert.AreEqual(cash + penalty, buyer.Board.cash);
            Assert.AreEqual(earned - penalty, seller.Board.cash);
        }
    }
}
