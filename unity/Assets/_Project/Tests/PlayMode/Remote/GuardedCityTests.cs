using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using Steel.PlayTests.Support;
using Steel.Presentation.UI.Boot;
using Steel.Services;
using Steel.Services.Remote;
using Steel.TestServers;
using UnityEngine;
using UnityEngine.TestTools;

namespace Steel.PlayTests.Remote
{
    /// <summary>The built city against a server that checks positions as the public one does: where the scene puts the
    /// player and the vehicles, and a walk at the game's own pace, are believed.</summary>
    public sealed class GuardedCityTests
    {
        private CareerProcess server;

        [TearDown] public void Close()
        {
            BootScreen.SignIn = null; BootStage.Close(); server?.Dispose(); server = null;
        }

        [UnityTest] public IEnumerator TheSceneAndAWalkAreBelievedByAServerThatChecksPositions()
        {
            server = new CareerProcess(); server.Start(guarded: true);
            var rules = new ResourceConfigSource();
            CareerClient Client() => new CareerClient(CareerEndpoint.Local(server.Port), server.Token);
            BootScreen.SignIn = account => OnlineCareer.Open(Client(), rules);
            yield return BootStage.OpenMenu(CareerSummary.Empty, null);
            BootScreen boot = BootStage.Screen;
            boot.Menu.Online.onClick.Invoke(); yield return null; yield return null;
            boot.Online.Token.text = server.Token; boot.Online.Go.onClick.Invoke();
            yield return BootStage.UntilCity();
            GameHost host = Object.FindObjectOfType<GameHost>();
            Assert.IsTrue(host.Online);

            var player = Object.FindObjectOfType<CharacterController>();
            Vector3 from = player.transform.position;
            for (float until = Time.time + 8; Time.time < until;)
            {
                player.enabled = false;
                player.transform.position += Vector3.forward * host.Layout.MoveSpeed * Time.deltaTime;
                player.enabled = true;
                yield return null;
                Assert.IsNull(host.SaveNotice, "The server refused what the scene reported");
            }
            Vector3 to = player.transform.position;
            Assert.Greater(Vector3.Distance(from, to), 15, "The player walked");

            Task<RemoteReply> seen = null;
            float limit = Time.realtimeSinceStartup + 30;
            while (Time.realtimeSinceStartup < limit && !(seen != null && seen.IsCompleted && Near(seen.Result, rules, to)))
            {
                if (seen == null || seen.IsCompleted) seen = Client().Open();
                yield return null;
            }
            Assert.IsTrue(seen.IsCompleted && Near(seen.Result, rules, to), "The server's career stands where the player walked to");
            Assert.IsNull(host.SaveNotice);
        }

        private static bool Near(RemoteReply reply, IConfigSource rules, Vector3 point)
        {
            GameView view = LocalGameService.Resume(rules, reply.save, reply.savedUtc, out _).View;
            return Vector2.Distance(new Vector2(view.PlayerX, view.PlayerZ), new Vector2(point.x, point.z)) < 3;
        }
    }
}
