using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using Steel.PlayTests.Support;
using Steel.Presentation.UI.Boot;
using Steel.Presentation.World.Town;
using Steel.Services;
using Steel.Services.Remote;
using Steel.TestServers;
using UnityEngine;
using UnityEngine.TestTools;

namespace Steel.PlayTests.Remote
{
    /// <summary>The whole game online: start menu, sign-in, the city, and a pick-up that ends up on the real server.</summary>
    public sealed class OnlinePlayTests
    {
        private CareerProcess server;

        [TearDown] public void Close()
        {
            BootScreen.SignIn = null; BootStage.Close(); server?.Dispose(); server = null;
        }

        private static IEnumerator Wait(System.Func<bool> done, float seconds, string what)
        {
            float limit = Time.realtimeSinceStartup + seconds;
            while (!done() && Time.realtimeSinceStartup < limit) yield return null;
            Assert.IsTrue(done(), what);
        }

        [UnityTest] public IEnumerator SigningInAtTheMenuPlaysTheCityOnTheServersCareer()
        {
            server = new CareerProcess(); server.Start();
            var rules = new ResourceConfigSource();
            CareerClient Client(string token) => new CareerClient(CareerEndpoint.Local(server.Port), token);
            BootScreen.SignIn = account => OnlineCareer.Open(Client(account.token), rules);
            yield return BootStage.OpenMenu(CareerSummary.Empty, null);
            BootScreen boot = BootStage.Screen;

            boot.Menu.Online.onClick.Invoke(); yield return null; yield return null;
            Assert.IsTrue(boot.Online.Screen.Visible);
            Assert.AreSame(boot.Online.Go.gameObject, BootStage.Under(boot.Online.Go), "The card covers the menu");
            boot.Online.Token.text = new string('x', 43); boot.Online.Go.onClick.Invoke();
            yield return Wait(() => boot.Login.Stage == LoginStage.Form, 20, "A wrong token returns to the form");
            StringAssert.Contains("Akun", boot.Online.Message.text);
            Assert.AreEqual(MenuStage.Menu, boot.Flow.Stage);

            boot.Online.Token.text = server.Token; boot.Online.Go.onClick.Invoke();
            yield return Wait(() => boot.Flow.Stage == MenuStage.Loading, 20, "Signed in");
            yield return BootStage.UntilCity();
            GameHost host = Object.FindObjectOfType<GameHost>();
            Assert.IsTrue(host.Online);

            SliceLocation pile = host.Layout.Piles[0].Position;
            host.Game.SetPosition(pile.X, pile.Z); host.Game.Inspect("p0"); host.Game.PickUp("p0");
            Assert.Greater(host.Game.View.CarriedKg, 0);
            Task<RemoteReply> seen = null;
            yield return Wait(() =>
            {
                if (seen == null || seen.IsCompleted && Carried(seen.Result, rules) <= 0) seen = Client(server.Token).Open();
                return seen.Status == TaskStatus.RanToCompletion && Carried(seen.Result, rules) > 0;
            }, 30, "The server's career carries the pile");
            Assert.AreEqual(Carried(seen.Result, rules), host.Game.View.CarriedKg, 1e-9);
            Assert.IsNull(host.SaveNotice);

            // the city is shared: a second player who signs in is seen walking in it, under his name
            Assert.IsEmpty(Object.FindObjectsOfType<Ghost>());
            Task<RemoteReply> neighbour = Client(server.Neighbour()).Open();
            yield return Wait(() => Object.FindObjectsOfType<Ghost>().Length == 1, 30, "The other player is shown");
            Ghost other = Object.FindObjectOfType<Ghost>();
            Assert.AreEqual("Hasan", other.Label.text);
            GameView start = LocalGameService.Resume(rules, neighbour.Result.save, neighbour.Result.savedUtc, out _).View;
            Assert.Less(Vector2.Distance(new Vector2(other.transform.position.x, other.transform.position.z),
                new Vector2(start.PlayerX, start.PlayerZ)), .5f, "He stands where the server saw him");
        }

        private static double Carried(RemoteReply reply, IConfigSource rules) =>
            LocalGameService.Resume(rules, reply.save, reply.savedUtc, out _).View.CarriedKg;
    }
}
