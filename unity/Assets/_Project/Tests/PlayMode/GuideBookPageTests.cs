using System.Collections;
using System.Linq;
using NUnit.Framework;
using Steel.Presentation.UI;
using Steel.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Steel.PlayTests
{
    /// <summary>The beginner's guide is one tap inside Buku usaha and never opens on its own.</summary>
    public sealed class GuideBookPageTests
    {
        [UnityTest] public IEnumerator GuideOpensOnlyOnRequestAndPagesThroughEveryTopic()
        {
            yield return SceneManagement();
            var hud = Object.FindObjectOfType<CityHud>();
            var root = hud.SafeRoot; var letter = root.Find("Surat Pak Darto"); var book = root.Find("Buku usaha");
            void Tap(Transform t) => ExecuteEvents.Execute(t.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);
            Tap(letter); Tap(letter);
            yield return new WaitForSecondsRealtime(.25f); yield return null;
            Assert.IsFalse(book.gameObject.activeSelf, "The guide never opens by itself");

            Tap(root.Find("Usaha")); Assert.IsTrue(book.gameObject.activeSelf);
            book.Find("PANDUAN").GetComponent<Button>().onClick.Invoke();
            bool Shown(string part) => book.GetComponentsInChildren<Text>().Any(t => t.text.Contains(part));
            Assert.IsTrue(Shown("/   Panduan pemula")); Assert.IsTrue(Shown("1/5  ·  Dari mana uang?"));
            Assert.IsTrue(Shown("satu tumpukan tiap 5 menit"));
            var next = book.GetComponentsInChildren<Button>()
                .Single(b => b.GetComponentInChildren<Text>().text == "TOPIK BERIKUTNYA");
            for (int topic = 1; topic < GuideBook.Topics.Length; topic++)
            {
                next.onClick.Invoke(); Assert.IsTrue(Shown(GuideBook.Heading(topic)));
            }
            next.onClick.Invoke(); Assert.IsTrue(Shown(GuideBook.Heading(0)));
            book.Find("TUTUP").GetComponent<Button>().onClick.Invoke();
            Assert.IsFalse(book.gameObject.activeSelf);
            yield return new WaitForSecondsRealtime(.25f); yield return null;
            StringAssert.DoesNotContain(GuideBook.Pointer, hud.PromptText.text, "The pointer goes away once read");
        }

        private static IEnumerator SceneManagement()
        {
            yield return SceneManager.LoadSceneAsync("City"); yield return null;
        }
    }
}
