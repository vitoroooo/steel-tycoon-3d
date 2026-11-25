using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.Services;
using Steel.Presentation.Player;

namespace Steel.PlayTests
{
    public sealed class CitySmokeTests
    {
        [UnityTest]
        public IEnumerator CityLoadsAndPickSortSellIncreasesCash()
        {
            yield return SceneManager.LoadSceneAsync("City");
            yield return null;
            var host = Object.FindObjectOfType<GameHost>();
            Assert.IsNotNull(host); Assert.IsNotNull(Object.FindObjectOfType<PlayerMover>());
            Assert.IsNotNull(Camera.main);
            var game = host.Game; var layout = host.Layout;
            double before = game.View.Cash;
            game.SetPosition(layout.Piles[0].Position.X, layout.Piles[0].Position.Z);
            game.Inspect("p0"); game.PickUp("p0");
            Assert.AreEqual(12, game.View.CarriedKg);
            game.SetPosition(layout.Home.X, layout.Home.Z); game.SortAtHome();
            game.SetPosition(layout.Factory.X, layout.Factory.Z);
            var offer = game.PreviewSale();
            Assert.AreEqual(before, game.View.Cash);
            game.ConfirmSale();
            Assert.Greater(game.View.Cash, before);
            Assert.AreEqual(before + offer.Revenue, game.View.Cash, .001);
            Assert.AreEqual(0, game.View.CarriedKg);
            yield return null;
        }
    }
}
