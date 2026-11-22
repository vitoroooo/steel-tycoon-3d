using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using Steel.Services;
using Steel.Services.Remote;
using Steel.TestServers;
using UnityEngine.TestTools;

namespace Steel.Tests.Remote
{
    /// <summary>One purse (PRD 12): what an online player buys on the market between players is paid from the cash
    /// of his career, and the scrap arrives in his yard.</summary>
    public sealed class OnePurseTests
    {
        private static IEnumerator Until(Task task)
        {
            while (!task.IsCompleted) yield return null;
            task.GetAwaiter().GetResult();
        }

        private static IEnumerator Play(IGameService game, double seconds)
        {
            var watch = Stopwatch.StartNew();
            for (double last = 0; last < seconds;)
            {
                yield return null;
                double now = watch.Elapsed.TotalSeconds;
                game.Advance(now - last); last = now;
            }
        }

        [UnityTest] public IEnumerator ALotBoughtOnTheMarketIsPaidFromTheCareerAndLandsInTheYard()
        {
            using (var server = new CareerProcess())
            {
                server.Start();
                var rules = new ResourceConfigSource();
                Task<OnlineCareer> opening = OnlineCareer.Open(new CareerClient(CareerEndpoint.Local(server.Port), server.Token), rules);
                yield return Until(opening);
                RemoteGameService game = opening.Result.Play(rules);
                double cash = game.View.Cash;

                var mine = new NetworkMarket(server.MarketPort);
                yield return Until(mine.Connect(server.Token));
                Assert.AreEqual((long)cash, mine.Board.cash, "Kas di pasar adalah kas karier.");
                Assert.AreEqual(0, mine.Board.stock);

                var neighbour = new NetworkMarket(server.MarketPort);
                yield return Until(neighbour.Connect(server.Neighbour()));
                yield return Until(neighbour.Sell(10, neighbour.Board.reference));
                yield return Until(mine.Refresh());
                long gross = mine.Board.lots[0].Gross;
                yield return Until(mine.Take(mine.Board.lots[0].id, false));
                Assert.AreEqual((long)cash - gross, mine.Board.cash);

                yield return Play(game, RemoteLink.Beat + 2);
                Assert.AreEqual(cash - gross, game.View.Cash, 1e-6, "Karier membayar lot itu.");
                Assert.AreEqual(1, game.Corrections);
                yield return Play(game, RemoteLink.Beat + 2);
                Assert.AreEqual(1, game.Corrections, "Setelah itu HP dan server kembali sepakat.");
                yield return Until(mine.Refresh());
                Assert.AreEqual(10, mine.Board.stock);
            }
        }
    }
}
