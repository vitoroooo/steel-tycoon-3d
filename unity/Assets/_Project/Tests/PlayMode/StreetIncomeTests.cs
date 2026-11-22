using System.Collections;
using System.Linq;
using NUnit.Framework;
using Steel.PlayTests.Support;
using Steel.Presentation.UI;
using Steel.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Steel.PlayTests
{
    /// <summary>"Kalau besi di pinggir jalan habis, dapat uang dari mana?" answered on the landscape HUD.</summary>
    public sealed class StreetIncomeTests
    {
        private static IEnumerator Readout() { yield return new WaitForSecondsRealtime(.25f); yield return null; }

        [UnityTest] public IEnumerator BareStreetIsExplainedAndTheFreeRoundBooksFromTheStartingCash()
        {
            yield return SceneManager.LoadSceneAsync("City"); yield return null;
            var host = Object.FindObjectOfType<GameHost>(); var hud = Object.FindObjectOfType<CityHud>();
            var g = host.Game; var letter = hud.SafeRoot.Find("Surat Pak Darto");
            void Tap(Transform t) => ExecuteEvents.Execute(t.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);
            Assert.AreEqual(500000, g.View.Cash);
            Tap(letter); Tap(letter); Assert.IsFalse(letter.gameObject.activeSelf);

            StreetSweep.Until(host, 1);
            var last = g.Income; OwnedFleet.PlacePlayer(host, last.SiteX, last.SiteZ);
            g.Inspect(g.Nearby.Id); hud.Act();
            StringAssert.Contains("Tumpukan terakhir di gang dipungut", hud.NoticeText.text);
            StreetSweep.Sell(host);
            Assert.Zero(g.Income.StreetSites); Assert.Greater(g.View.Cash, 500000);
            Assert.IsFalse(g.View.GrantCollected);
            yield return Readout();
            Assert.IsTrue(letter.gameObject.activeSelf, "Pak Darto explains the bare street once");
            StringAssert.Contains("Warga menaruh rongsok baru tiap 10 menit",
                letter.GetComponentInChildren<Text>().text);
            StringAssert.Contains("PESAN RUTE", letter.GetComponentInChildren<Text>().text);
            StringAssert.Contains("ambil hadiah setoran pertama", hud.RouteText.text);
            StringAssert.Contains("pesan rute gratis 20 kg (30 menit)", IncomeHint.Text(g.Income, g.View, host.Layout));
            Tap(letter); Assert.IsTrue(g.StreetTipRead);
            yield return Readout(); Assert.IsFalse(letter.gameObject.activeSelf);

            double cash = g.View.Cash;
            OwnedFleet.PlacePlayer(host, host.Layout.Home.X, host.Layout.Home.Z);
            Assert.IsTrue(g.Cart.Held); hud.Act();
            var desk = hud.SafeRoot.Find("Meja rute"); Assert.IsTrue(desk.gameObject.activeSelf);
            var buttons = desk.GetComponentsInChildren<Button>();
            buttons.Single(b => b.name == "PESAN RUTE").onClick.Invoke();
            Assert.AreEqual(1, g.Route("cart", 0, false).Queue, "The held, empty gerobak is parked and sent");
            Assert.IsFalse(g.Cart.Held); Assert.AreEqual(cash, g.View.Cash);
            g.CatchUp(1800);
            Assert.AreEqual(20, g.Income.DepotKg); Assert.GreaterOrEqual(g.Income.StreetSites, 3);
            buttons.Single(b => b.name == "AMBIL KIRIMAN").onClick.Invoke();
            buttons.Single(b => b.name == "KEMBALI").onClick.Invoke();
            Assert.AreEqual(15, g.View.CarriedKg);
            StreetSweep.Sell(host); Assert.Greater(g.View.Cash, cash);
        }
    }
}
