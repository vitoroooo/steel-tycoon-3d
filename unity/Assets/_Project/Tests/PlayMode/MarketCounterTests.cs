using System.Collections;
using System.Linq;
using NUnit.Framework;
using Steel.PlayTests.Support;
using Steel.Presentation.UI;
using Steel.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Steel.PlayTests
{
    public sealed class MarketCounterTests
    {
        [UnityTest] public IEnumerator LapakInvoiceRequiresConfirmationAndRemovesOnlyAcceptedLots()
        {
            yield return SceneManager.LoadSceneAsync("City"); yield return null;
            var host = Object.FindObjectOfType<GameHost>(); var game = host.Game;
            var hud = Object.FindObjectOfType<CityHud>();
            OwnedFleet.PlacePlayer(host, game.Cart.X, game.Cart.Z); game.GrabCart();
            foreach (int i in new[] { 0, 8 })
            {
                var lot = host.Layout.Piles[i]; OwnedFleet.PlacePlayer(host, lot.Position.X, lot.Position.Z);
                game.Inspect(lot.Id); game.PickUp(lot.Id);
            }
            OwnedFleet.PlacePlayer(host,host.Layout.Home.X,host.Layout.Home.Z);
            game.SortAtHome(); hud.Act();
            var panel = hud.SafeRoot.Find("Papan harga lokal"); Assert.IsTrue(panel.gameObject.activeSelf);
            var submit = panel.GetComponentsInChildren<Button>().Single(b => b.name == "PERIKSA NOTA");
            var before = game.View.Cash; submit.onClick.Invoke(); Assert.AreEqual(before,game.View.Cash);
            submit.onClick.Invoke(); Assert.Greater(game.View.Cash,before);
            Assert.AreEqual(24,game.View.CarriedKg); Assert.IsFalse(panel.gameObject.activeSelf);
        }
    }
}
