using System.Collections;
using System.Linq;
using NUnit.Framework;
using Steel.Core.Trade.Town;
using Steel.PlayTests.Support;
using Steel.Presentation.UI;
using Steel.Presentation.World;
using Steel.Presentation.World.Town;
using Steel.Services;
using UnityEngine;
using UnityEngine.TestTools;

namespace Steel.PlayTests.Town
{
    /// <summary>Another player's buying board as it stands in the built city (PRD 5.2).</summary>
    public sealed class PlayerYardSceneTests
    {
        [UnityTest] public IEnumerator AnotherPlayersBoardShowsHisPricesAndBuysTheLoadAtItsPost()
        {
            yield return OwnedFleet.LoadWealthyCity(500000);
            var host = Object.FindObjectOfType<GameHost>(); var hud = Object.FindObjectOfType<CityHud>();
            var game = (LocalGameService)host.Game;
            PlotBoard board = Object.FindObjectsOfType<PlotBoard>().First(b => b.GetComponent<PlayerPlot>().Number == 2);
            yield return new WaitForSecondsRealtime(1.2f);
            StringAssert.Contains("DISEWAKAN", board.Sign.text);

            game.PostYards(new[] { new PlayerYard { owner = "Budi", plot = 2, bids = new double[] { 5200, 4100, 3000, 2500 }, roomKg = 60000 } },
                new[] { 300e6 });
            yield return new WaitForSecondsRealtime(1.2f);
            StringAssert.Contains("LAPAK BUDI", board.Sign.text);
            StringAssert.Contains("5.200", board.Sign.text);

            PlayerYardView yard = game.PlayerYards.Single();
            var at = new Vector3(yard.X, 0, yard.Z);
            Assert.IsFalse(Physics.CheckSphere(at + Vector3.up * 1.2f, .4f, Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore), "Something solid stands on the board's post at " + at);
            Assert.AreNotEqual("nothing", GroundProbe.Under(at + Vector3.up * 2));

            var pile = game.Income; OwnedFleet.PlacePlayer(host, pile.SiteX, pile.SiteZ);
            string id = game.Nearby.Id; game.Inspect(id); game.PickUp(id);
            OwnedFleet.PlacePlayer(host, host.Layout.Home.X, host.Layout.Home.Z); game.SortAtHome();
            OwnedFleet.PlacePlayer(host, yard.X, yard.Z);
            Assert.AreEqual("playeryard", game.Nearby.Id);
            double before = game.View.Cash;
            hud.Act();
            StringAssert.Contains("Disetor ke lapak Budi", hud.NoticeText.text);
            Assert.Greater(game.View.Cash, before);
            Assert.Zero(game.View.CarriedKg);
        }
    }
}
