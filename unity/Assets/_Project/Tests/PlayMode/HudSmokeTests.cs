using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.Services;
using Steel.Presentation.Player;
using Steel.Presentation.UI;
using Steel.Presentation.Vehicles;

namespace Steel.PlayTests
{
    public sealed class HudSmokeTests
    {
        private GameHost host;
        private CityHud hud;
        private PickupDriver driver;
        private IEnumerator Load()
        {
            yield return SceneManager.LoadSceneAsync("City"); yield return null;
            host = Object.FindObjectOfType<GameHost>(); hud = Object.FindObjectOfType<CityHud>();
            driver = Object.FindObjectOfType<PickupDriver>();
        }
        private void PutActor(float x, float z)
        {
            var actor = Object.FindObjectOfType<PlayerMover>();
            var controller = actor.GetComponent<CharacterController>(); controller.enabled = false;
            actor.transform.position = new Vector3(x, host.Layout.SpawnY, z); controller.enabled = true;
            host.Game.SetPosition(x, z);
        }
        private void AssertOneContext() => Assert.AreEqual(1,
            hud.GetComponentsInChildren<OnScreenButton>().Length + hud.GetComponentsInChildren<Button>().Length);
        [UnityTest] public IEnumerator SingleContextHandlesEmptyVehicleAndBlockedExit()
        {
            yield return Load();
            PutActor(40, -30); yield return null;
            AssertOneContext(); Assert.AreEqual("CARI RONGSOK", hud.ActionText.text);
            double cash = host.Game.View.Cash; hud.Act(); Assert.AreEqual(cash, host.Game.View.Cash);
            driver.GetComponent<Rigidbody>().position = new Vector3(0, .09f, 30);
            host.Game.ReportVehicle(0, 30, 0);
            float end = Time.fixedTime + .12f;
            while (Time.fixedTime < end) yield return new WaitForFixedUpdate();
            PutActor(1.7f, 30); yield return null;
            AssertOneContext(); Assert.AreEqual("MASUK PIKAP", hud.ActionText.text);
            hud.Act(); yield return null;
            AssertOneContext(); Assert.AreEqual("KELUAR PIKAP", hud.ActionText.text);
            Assert.AreEqual(1000, host.Game.View.CapacityKg);
            host.Game.ReportVehicle(0, 30, 1); hud.Act();
            Assert.IsTrue(host.Game.Vehicle.Occupied); StringAssert.Contains("Hentikan", hud.NoticeText.text);
            host.Game.ReportVehicle(0, 30, 0); hud.Act(); yield return null;
            AssertOneContext(); Assert.IsFalse(host.Game.Vehicle.Occupied, hud.NoticeText.text);
            Assert.AreEqual(50, host.Game.View.CapacityKg);
        }
        [UnityTest] public IEnumerator PilePriorityAndBottomRegionsStaySeparate()
        {
            yield return Load();
            var pile = host.Layout.Piles[0]; PutActor(pile.Position.X, pile.Position.Z); yield return null;
            AssertOneContext(); Assert.AreEqual("PERIKSA", hud.ActionText.text);
            hud.Act(); yield return null; Assert.AreEqual("PUNGUT", hud.ActionText.text);
            Canvas.ForceUpdateCanvases();
            var info = RectTransformUtility.CalculateRelativeRectTransformBounds(hud.SafeRoot, hud.SafeRoot.Find("Petunjuk"));
            var help = RectTransformUtility.CalculateRelativeRectTransformBounds(hud.SafeRoot, hud.RouteText.rectTransform);
            var notice = RectTransformUtility.CalculateRelativeRectTransformBounds(hud.SafeRoot, hud.NoticeText.rectTransform);
            Assert.Less(help.max.y, info.min.y); Assert.Greater(notice.min.y, info.max.y);
        }
    }
}
