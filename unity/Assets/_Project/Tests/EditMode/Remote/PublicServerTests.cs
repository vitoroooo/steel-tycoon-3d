using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using Steel.Services;
using Steel.Services.Remote;
using UnityEngine.TestTools;

namespace Steel.Tests.Remote
{
    /// <summary>The game's own client against the deployed server, over HTTPS. Runs only where the operator left an
    /// address and a test account's token in UserSettings/akun-online.json (never committed); skipped elsewhere.</summary>
    public sealed class PublicServerTests
    {
        private static IEnumerator Until(Task task)
        {
            while (!task.IsCompleted) yield return null;
            task.GetAwaiter().GetResult();
        }

        [UnityTest] public IEnumerator TheDeployedServerKeepsTheCareerAndAgreesWithThePhone()
        {
            OnlineAccount account = OnlineAccount.Read("UserSettings");
            if (!account.Known) Assert.Ignore("Tidak ada akun uji server publik di UserSettings/akun-online.json.");
            var rules = new ResourceConfigSource();
            Task<OnlineCareer> opening = OnlineCareer.Open(account.Client(), rules);
            yield return Until(opening);
            RemoteGameService game = opening.Result.Play(rules);
            double cash = game.View.Cash, hours = game.View.GameHours;

            game.SetPosition(game.View.PlayerX + 1, game.View.PlayerZ);
            var watch = Stopwatch.StartNew();
            for (double last = 0; last < 8;)
            {
                yield return null;
                double now = watch.Elapsed.TotalSeconds;
                game.Advance(now - last); last = now;
            }
            Assert.IsNull(game.Notice, "Sambungan ke server tidak terputus.");
            Assert.AreEqual(0, game.Corrections, "Server menghitung hasil yang sama dengan HP.");

            Task<OnlineCareer> again = OnlineCareer.Open(account.Client(), rules);
            yield return Until(again);
            RemoteGameService back = again.Result.Play(rules);
            Assert.AreEqual(cash, back.View.Cash, 1e-6);
            Assert.Greater(back.View.GameHours, hours + 5 / 600d, "Jam karier berjalan di server.");
            Assert.AreEqual(game.View.PlayerX, back.View.PlayerX, 1e-3f, "Posisi terakhir tersimpan di server.");
        }
    }
}
