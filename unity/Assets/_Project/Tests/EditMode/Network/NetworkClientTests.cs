using System;
using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using Steel.Services;
using UnityEngine.TestTools;

namespace Steel.Tests.Network
{
    public sealed class NetworkClientTests
    {
        [UnityTest]
        public IEnumerator SettleBetweenAuthenticatedUnityClients()
        {
            using (var server = new NetworkProcess())
            {
                server.Start();
                Task flow = Trade(server);
                while (!flow.IsCompleted) yield return null;
                flow.GetAwaiter().GetResult();
            }
        }

        private static async Task Trade(NetworkProcess server)
        {
            var seller = new NetworkMarket(server.Port);
            var buyer = new NetworkMarket(server.Port);
            Assert.IsNull(buyer.Board);
            Assert.Throws<InvalidOperationException>(() => { buyer.Take(1,false); });
            await seller.Connect(server.IssueToken(true));
            await buyer.Connect(server.IssueToken(false));
            var before = seller.Board;
            var wallet = buyer.Board;
            Assert.AreNotEqual(before.id, wallet.id);
            await seller.Sell(5, before.reference);
            await buyer.Refresh();
            Assert.AreEqual(1, buyer.Board.lots.Length);
            var lot = buyer.Board.lots[0];
            Assert.AreEqual(before.id, lot.seller);
            Assert.AreEqual(5, lot.kg);
            await buyer.Take(lot.id,false);
            await seller.Refresh();
            long tax = (lot.Gross * 2 + 50) / 100;
            Assert.AreEqual(before.cash + lot.Gross - tax, seller.Board.cash);
            Assert.AreEqual(wallet.cash - lot.Gross, buyer.Board.cash);
            Assert.AreEqual(before.stock - 5, seller.Board.stock);
            Assert.AreEqual(wallet.stock + 5, buyer.Board.stock);
            Assert.AreEqual(0, buyer.Board.lots.Length);
            await ContractFlow.Run(seller, buyer);
            await GuildFlow.Run(seller, buyer);
            buyer.Disconnect();
            Assert.IsFalse(buyer.HasSession);
            Assert.IsNull(buyer.Board);
        }
    }
}
