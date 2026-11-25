using System.Collections;
using NUnit.Framework;
using Steel.PlayTests.Support;
using Steel.Presentation.UI.Boot;
using Steel.Services;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Steel.PlayTests.Boot
{
    public sealed class MenuThumbTests
    {
        private static readonly CareerSummary Saved = CareerSummary.Saved(3400000, 12, 2, 1);
        private int profile;
        private GraphicsOption Profiles() =>
            new GraphicsOption(() => new[] { "Seimbang", "Ringan", "Tajam" }[profile], () => profile = (profile + 1) % 3);
        [SetUp] public void Reset() => profile = 0;
        [TearDown] public void Close() => BootStage.Close();

        [UnityTest] public IEnumerator EveryButtonIsThumbSizedInTheLowerHalfAndUncovered()
        {
            yield return BootStage.OpenMenu(Saved, Profiles());
            var menu = BootStage.Screen.Menu; Rect safe = BootStage.Screen.SafeRoot.rect;
            foreach (var button in new[] { menu.Play, menu.NewCareer, menu.Graphics, menu.Online })
            {
                Rect area = BootStage.Area(button);
                Assert.GreaterOrEqual(area.width, 88f, button.name); Assert.GreaterOrEqual(area.height, 88f, button.name);
                Assert.Less(area.yMax, safe.height / 2, button.name + " sits in the lower half");
                Assert.GreaterOrEqual(area.xMin, 0f); Assert.LessOrEqual(area.xMax, safe.width);
                Assert.AreSame(button.gameObject, BootStage.Under(button), button.name + " is what the thumb hits");
            }
            Assert.Greater(BootStage.Area(menu.Play).xMin, safe.width / 2, "Play under the right thumb");
            Assert.Less(BootStage.Area(menu.NewCareer).xMax, safe.width / 2, "Starting over under the left thumb");
            Assert.Less(BootStage.Area(menu.Graphics).xMax, BootStage.Area(menu.Play).xMin);
            Assert.IsFalse(BootStage.Area(menu.NewCareer).Overlaps(BootStage.Area(menu.Graphics)));
            foreach (var below in new[] { menu.NewCareer, menu.Graphics, menu.Play })
                Assert.IsFalse(BootStage.Area(menu.Online).Overlaps(BootStage.Area(below)), below.name);
            Assert.AreEqual("LANJUTKAN", menu.Play.GetComponentInChildren<Text>().text);
            StringAssert.Contains("Hari ke-12", menu.Card.text); StringAssert.Contains("Rp 3.400.000", menu.Card.text);
        }

        [UnityTest] public IEnumerator GraphicsButtonStepsThroughTheProfiles()
        {
            yield return BootStage.OpenMenu(Saved, Profiles());
            var button = BootStage.Screen.Menu.Graphics; var caption = button.GetComponentInChildren<Text>();
            Assert.AreEqual("Grafis: Seimbang  ›", caption.text);
            button.onClick.Invoke(); Assert.AreEqual("Grafis: Ringan  ›", caption.text); Assert.AreEqual(1, profile);
            button.onClick.Invoke(); button.onClick.Invoke(); Assert.AreEqual("Grafis: Seimbang  ›", caption.text);
            Assert.AreEqual(MenuStage.Menu, BootStage.Screen.Flow.Stage);
        }

        [UnityTest] public IEnumerator EveryCaptionStaysOnOneLine()
        {
            foreach (var career in new[] { Saved, CareerSummary.Empty, CareerSummary.Unreadable })
            {
                yield return BootStage.OpenMenu(career, Profiles());
                var boot = BootStage.Screen;
                for (int step = 0; step < 3; step++, boot.Menu.Graphics.onClick.Invoke())
                {
                    Canvas.ForceUpdateCanvases();
                    foreach (var caption in boot.GetComponentsInChildren<Text>(true))
                        if (caption.GetComponentInParent<Button>() && caption.gameObject.activeInHierarchy)
                            Assert.AreEqual(1, caption.cachedTextGenerator.lineCount, caption.text);
                }
            }
        }

        [UnityTest] public IEnumerator MenuWithoutProfilesOrSavedCareerShowsOnlyPlayAndOnline()
        {
            yield return BootStage.OpenMenu(CareerSummary.Empty, null);
            var menu = BootStage.Screen.Menu;
            Assert.IsNull(menu.Graphics); Assert.IsNull(menu.NewCareer);
            CollectionAssert.AreEquivalent(new[] { menu.Play, menu.Online }, menu.Screen.GetComponentsInChildren<Button>());
        }

        [UnityTest] public IEnumerator NewCareerAsksFirstAndBatalKeepsTheCareer()
        {
            yield return BootStage.OpenMenu(Saved, Profiles());
            var boot = BootStage.Screen;
            boot.Menu.NewCareer.onClick.Invoke(); yield return null; yield return null;
            Assert.AreEqual(MenuStage.Confirm, boot.Flow.Stage); Assert.IsTrue(boot.Confirm.Screen.Visible);
            StringAssert.Contains("Hari ke-12", boot.Confirm.Body.text);
            Assert.AreNotSame(boot.Menu.Play.gameObject, BootStage.Under(boot.Menu.Play), "Dialog covers the menu");
            Assert.AreSame(boot.Confirm.No.gameObject, BootStage.Under(boot.Confirm.No));
            Assert.IsFalse(BootStage.Area(boot.Confirm.Yes).Overlaps(BootStage.Area(boot.Menu.NewCareer)),
                "A double tap on Karier baru cannot confirm it");
            Assert.Greater(BootStage.Area(boot.Confirm.No).xMin, BootStage.Area(boot.Confirm.Yes).xMax);

            boot.Confirm.No.onClick.Invoke(); yield return null;
            Assert.AreEqual(MenuStage.Menu, boot.Flow.Stage); Assert.IsFalse(boot.Confirm.Screen.Visible);
            boot.Menu.NewCareer.onClick.Invoke(); boot.Confirm.Yes.onClick.Invoke(); yield return null;
            Assert.AreEqual(MenuStage.Loading, boot.Flow.Stage); Assert.IsTrue(boot.Flow.Fresh);
            yield return BootStage.UntilCity();
            Assert.IsFalse(CareerLaunch.TakeFresh(), "The city consumed the choice; it cannot repeat");
        }
    }
}
