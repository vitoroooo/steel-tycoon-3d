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
    public sealed class GuideTests
    {
        [UnityTest] public IEnumerator TwoPagesForecastAndShelterWorkFromTheLandscapeHud()
        {
            yield return SceneManager.LoadSceneAsync("City"); yield return null;
            var host = Object.FindObjectOfType<GameHost>(); var hud = Object.FindObjectOfType<CityHud>();
            var root = hud.SafeRoot; var letter = root.Find("Surat Pak Darto");
            void Tap(Transform t) => ExecuteEvents.Execute(t.gameObject,new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerClickHandler);
            Tap(letter); Assert.AreEqual(1,host.Game.Story); Tap(letter); Assert.IsFalse(letter.gameObject.activeSelf);
            var lot = host.Layout.Piles[0]; OwnedFleet.PlacePlayer(host,lot.Position.X,lot.Position.Z);
            host.Game.Inspect(lot.Id); host.Game.PickUp(lot.Id);
            OwnedFleet.PlacePlayer(host,host.Layout.Home.X,host.Layout.Home.Z);
            Tap(root.Find("Usaha")); var book = root.Find("Buku usaha"); Assert.IsTrue(book.gameObject.activeSelf);
            var next = book.Find("BERIKUTNYA").GetComponent<Button>();
            for (int p = 0; p < 3; p++) next.onClick.Invoke();
            Assert.IsTrue(host.Game.ReadWeather);
            book.GetComponentsInChildren<Button>().Single(b => b.GetComponentInChildren<Text>().text=="Titip di teras")
                .onClick.Invoke();
            Assert.IsTrue(host.Game.StoredUnderRoof); Assert.AreEqual(12,host.Game.Weather(0).CoveredKg);
            Assert.Zero(host.Game.View.CarriedKg);
            book.Find("DAFTAR ISI").GetComponent<Button>().onClick.Invoke();
            var index = book.Find("Daftar halaman"); Assert.IsTrue(index.gameObject.activeSelf);
            index.Find("Pabrik baja").GetComponent<Button>().onClick.Invoke();
            Assert.IsFalse(index.gameObject.activeSelf);
            Assert.IsTrue(book.GetComponentsInChildren<Text>().Any(t => t.text.Contains("AMDAL aktif")));
            foreach (var section in new[] {"Perawatan alat","Keuangan","Regu & warga","Pasar jaringan","Kontrak langsung","Asosiasi","Uji performa HP"})
            {
                book.Find("DAFTAR ISI").GetComponent<Button>().onClick.Invoke();
                if (!index.Find(section).gameObject.activeInHierarchy)
                    index.Find("HALAMAN LAIN").GetComponent<Button>().onClick.Invoke();
                index.Find(section).GetComponent<Button>().onClick.Invoke();
                Assert.IsTrue(book.GetComponentsInChildren<Text>().Any(t => t.text.EndsWith("/   "+section)));
            }
            book.Find("Pilihan 0").GetComponent<Button>().onClick.Invoke();
            Assert.IsFalse(book.gameObject.activeSelf);
        }
    }
}
