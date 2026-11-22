using System.Collections;
using NUnit.Framework;
using Steel.PlayTests.Support;
using Steel.Presentation.UI;
using Steel.Presentation.UI.Boot;
using Steel.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Steel.PlayTests.Boot
{
    public sealed class BootFlowTests
    {
        [TearDown] public void Close() => BootStage.Close();

        [Test] public void PlayerOpensOnTheBootSceneNotTheCity()
        {
            StringAssert.EndsWith("/Boot.unity", SceneUtility.GetScenePathByBuildIndex(0));
            StringAssert.EndsWith("/City.unity", SceneUtility.GetScenePathByBuildIndex(1));
        }

        [UnityTest] public IEnumerator SplashThenMenuThenCityOnlyAfterThePlayerChooses()
        {
            yield return BootStage.LoadScene();
            var boot = BootStage.Screen;
            Assert.IsNotNull(boot, "Boot scene carries the start screens");
            Assert.IsNull(Object.FindObjectOfType<GameHost>(), "No game is running behind the splash");
            Assert.AreEqual(MenuStage.Splash, boot.Flow.Stage);
            Assert.IsTrue(boot.Splash.Screen.Visible); Assert.IsFalse(boot.Menu.Screen.gameObject.activeSelf);
            StringAssert.Contains(BootTheme.Load().title, boot.Splash.Screen.GetComponentInChildren<Text>().text);

            boot.Splash.Surface.onClick.Invoke(); yield return null;
            Assert.AreEqual(MenuStage.Menu, boot.Flow.Stage);
            Assert.IsTrue(boot.Menu.Screen.Visible); Assert.IsFalse(boot.Splash.Screen.Visible);
            Assert.AreEqual("MULAI KARIER", boot.Menu.Play.GetComponentInChildren<Text>().text);
            Assert.IsNull(boot.Menu.NewCareer, "Nothing to replace on a first career");
            for (int i = 0; i < 30; i++) yield return null;
            Assert.AreEqual(BootScreen.Scene, SceneManager.GetActiveScene().name, "Menu waits for the player");

            boot.Menu.Play.onClick.Invoke(); yield return null;
            Assert.IsTrue(boot.Loading.Screen.Visible); Assert.IsFalse(boot.Menu.Screen.Visible);
            yield return BootStage.UntilCity();
            Assert.AreEqual(BootScreen.City, SceneManager.GetActiveScene().name);
            Assert.IsNotNull(Object.FindObjectOfType<GameHost>().Game);
            Assert.IsNotNull(Object.FindObjectOfType<CityHud>());
            Assert.IsNull(Object.FindObjectOfType<BootScreen>(), "Start screens are gone in the city");
        }

        [UnityTest] public IEnumerator SplashMovesOnByItself()
        {
            yield return BootStage.LoadScene();
            Assert.AreEqual(MenuStage.Splash, BootStage.Screen.Flow.Stage);
            yield return new WaitForSecondsRealtime(BootTheme.Load().SplashSeconds + .5f);
            Assert.AreEqual(MenuStage.Menu, BootStage.Screen.Flow.Stage);
        }
    }
}
