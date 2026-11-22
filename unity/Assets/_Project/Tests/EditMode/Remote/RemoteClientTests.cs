using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using Steel.TestServers;
using Steel.Services;
using Steel.Services.Remote;
using UnityEngine.TestTools;

namespace Steel.Tests.Remote
{
    /// <summary>The game's own client against the real career server over HTTP.</summary>
    public sealed class RemoteClientTests
    {
        private static IEnumerator Until(Task task)
        {
            while (!task.IsCompleted) yield return null;
            task.GetAwaiter().GetResult();
        }

        /// <summary>Lets real time pass on the phone, as frames do.</summary>
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

        [UnityTest] public IEnumerator APickedUpPileIsOnTheServerAndComesBackAfterSigningInAgain()
        {
            using (var server = new CareerProcess())
            {
                server.Start();
                var rules = new ResourceConfigSource();
                var client = new CareerClient(CareerEndpoint.Local(server.Port), server.Token);
                Task<OnlineCareer> opening = OnlineCareer.Open(client, rules);
                yield return Until(opening);
                Assert.AreEqual(CareerState.Saved, opening.Result.Summary.State);
                RemoteGameService game = opening.Result.Play(rules);

                SliceLocation pile = new CityLayout(rules).Piles[0].Position;
                game.SetPosition(pile.X, pile.Z); game.Inspect("p0"); game.PickUp("p0");
                double carried = game.View.CarriedKg;
                Assert.Greater(carried, 0);
                yield return Play(game, 2);
                Assert.IsNull(game.Notice);
                Assert.AreEqual(0, game.Corrections, "Server menghitung hasil yang sama dengan HP.");

                Task<OnlineCareer> again = OnlineCareer.Open(client, rules);
                yield return Until(again);
                Assert.AreEqual(carried, again.Result.Play(rules).View.CarriedKg, 1e-9);
            }
        }

        [UnityTest] public IEnumerator AnUnknownTokenIsRefusedInTheServersWords()
        {
            using (var server = new CareerProcess())
            {
                server.Start();
                var client = new CareerClient(CareerEndpoint.Local(server.Port), new string('x', 43));
                Task<RemoteReply> opening = client.Open();
                while (!opening.IsCompleted) yield return null;
                var refused = Assert.Throws<InvalidOperationException>(() => opening.GetAwaiter().GetResult());
                StringAssert.Contains("Akun", refused.Message);
            }
        }
    }
}
