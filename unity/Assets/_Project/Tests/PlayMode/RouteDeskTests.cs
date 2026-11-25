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
    public sealed class RouteDeskTests
    {
        [UnityTest] public IEnumerator BookAndRetrieveAtTheLapak()
        {
            yield return SceneManager.LoadSceneAsync("City"); yield return null;
            var host = Object.FindObjectOfType<GameHost>(); var hud = Object.FindObjectOfType<CityHud>();
            OwnedFleet.PlacePlayer(host, host.Layout.Home.X, host.Layout.Home.Z);
            hud.Act();
            var panel = hud.SafeRoot.Find("Meja rute");
            Assert.IsTrue(panel.gameObject.activeSelf);
            var buttons = panel.GetComponentsInChildren<Button>();
            buttons.Single(b => b.name == "PESAN RUTE").onClick.Invoke();
            Assert.AreEqual(1, host.Game.Route("cart", 0, false).Queue);
            host.Game.CatchUp(1800);
            buttons.Single(b => b.name == "AMBIL KIRIMAN").onClick.Invoke();
            buttons.Single(b => b.name == "KEMBALI").onClick.Invoke();
            Assert.IsFalse(panel.gameObject.activeSelf);
            Assert.AreEqual(15, host.Game.View.CarriedKg);
            hud.Act(); Assert.IsTrue(host.Game.View.LoadSorted);
        }
    }
}
