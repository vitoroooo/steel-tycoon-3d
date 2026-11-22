using System.Collections;
using System.Linq;
using NUnit.Framework;
using Steel.PlayTests.Support;
using Steel.Presentation.UI;
using Steel.Presentation.World.Street;
using Steel.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Steel.PlayTests.Street
{
    /// <summary>Doors that sell scrap, in the built city: each stands where the game deals, and the screen buys with real cash.</summary>
    public sealed class DoorDealTests
    {
        [UnityTest] public IEnumerator EveryDoorHasItsSellerOnTheGroundWhereTheGameDeals()
        {
            yield return SceneManager.LoadSceneAsync("City"); yield return null;
            var game = Object.FindObjectOfType<GameHost>().Game;
            var markers = Object.FindObjectsOfType<SellerMarker>();
            Assert.AreEqual(game.SellerCount, markers.Length, "One heap of goods per door in Sellers.json");
            var wrong = new System.Collections.Generic.List<string>();
            for (int i = 0; i < game.SellerCount; i++)
            {
                var door = game.Seller("s" + i);
                var seller = GameObject.Find("Penjual " + door.Id);
                Assert.IsNotNull(seller, door.Id + " has nobody selling");
                Vector3 at = seller.transform.position;
                Assert.Less(Vector2.Distance(new Vector2(at.x, at.z), new Vector2(door.X, door.Z)), .1f, door.Id + " stands elsewhere");
                if (Mathf.Abs(GroundProbe.Gap(at)) > .08f) wrong.Add(door.Id + " " + GroundProbe.Gap(at).ToString("0.00") + " at " + at + " on " + GroundProbe.Under(at));
            }
            Assert.IsEmpty(wrong, "Sellers floating or sunk:\n" + string.Join("\n", wrong));
        }

        [UnityTest] public IEnumerator ThePlayerLooksWeighsBargainsAndPaysAtADoor()
        {
            yield return SceneManager.LoadSceneAsync("City"); yield return null;
            var host = Object.FindObjectOfType<GameHost>(); var game = host.Game;
            var hud = Object.FindObjectOfType<CityHud>();
            var door = game.Seller("s0");
            OwnedFleet.PlacePlayer(host, door.X + 1.2f, door.Z); yield return null;
            Assert.AreEqual("seller", game.Nearby.Id);
            hud.Act();
            var panel = hud.SafeRoot.Find("Beli di tempat"); Assert.IsTrue(panel.gameObject.activeSelf);
            Button Key(string name) => panel.GetComponentsInChildren<Button>().Single(b => b.name == name);
            double cash = game.View.Cash;
            Key("BAYAR & MUAT").onClick.Invoke();
            Assert.AreEqual(cash, game.View.Cash, "Goods nobody looked at are not paid for");
            Key("LIHAT BARANG").onClick.Invoke(); Key("TIMBANG").onClick.Invoke();
            Assert.IsTrue(game.Seller(door.Id).Inspected); Assert.IsTrue(game.Seller(door.Id).Weighed);
            Key("−").onClick.Invoke(); Key("TAWAR").onClick.Invoke();
            double total = game.Seller(door.Id).Total;
            Key("BAYAR & MUAT").onClick.Invoke();
            Assert.AreEqual(cash - total, game.View.Cash); Assert.Greater(game.View.CarriedKg, 0);
            Assert.IsFalse(panel.gameObject.activeSelf);
        }
    }
}
