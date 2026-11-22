using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.PlayTests.Support;
using Steel.Presentation.UI;
using Steel.Services;
using Object = UnityEngine.Object;

namespace Steel.PlayTests.Vehicles
{
    public sealed class FirstSaleRideTests
    {
        [UnityTest]
        public IEnumerator FirstSetoranFundsARealDealerPurchase()
        {
            yield return SceneManager.LoadSceneAsync("City");
            yield return null;
            var host = Object.FindObjectOfType<GameHost>();
            var hud = Object.FindObjectOfType<CityHud>();
            var game = host.Game;
            Assert.AreEqual(500000, game.View.Cash);
            Assert.Throws<InvalidOperationException>(() => game.ClaimDealerGrant());
            var pile = host.Layout.Piles[0].Position;
            OwnedFleet.PlacePlayer(host, pile.X, pile.Z);
            yield return null;
            hud.Act(); SortGesture.Drop(hud, "HMS 1"); hud.Act();
            Assert.AreEqual(12, game.View.CarriedKg);
            var lapak = host.Layout.Home;
            OwnedFleet.PlacePlayer(host, lapak.X, lapak.Z);
            hud.Act(); SortGesture.Drop(hud, "HMS 1");
            var factory = host.Layout.Factory;
            OwnedFleet.PlacePlayer(host, factory.X, factory.Z);
            hud.Act(); hud.Cancel();
            Assert.IsFalse(game.View.GrantReady, "An unconfirmed sale cannot unlock the grant");
            hud.Act(); hud.Confirm();
            var afterSale = game.View.Cash;
            var bike = game.VehicleOf(FleetCatalog.Triseda);
            Assert.Less(afterSale, bike.Price);
            Assert.Throws<InvalidOperationException>(() => game.ClaimDealerGrant(), "Not at the dealer");
            OwnedFleet.PlacePlayer(host, bike.X, bike.Z);
            Assert.AreEqual("grant", game.Nearby.Id);
            hud.Act();
            Assert.AreEqual(afterSale + 12000000, game.View.Cash);
            Assert.Throws<InvalidOperationException>(() => game.ClaimDealerGrant(), "Already collected");
            hud.Act();
            Assert.IsTrue(game.VehicleOf(bike.Id).Owned);
            Assert.AreEqual(afterSale, game.View.Cash, .001);
            hud.Act();
            Assert.IsTrue(game.VehicleOf(bike.Id).Occupied);
            Assert.AreEqual(300, game.View.CapacityKg);
        }
    }
}
