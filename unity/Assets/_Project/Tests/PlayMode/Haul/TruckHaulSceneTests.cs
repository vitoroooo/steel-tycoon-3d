using System.Collections;
using System.Linq;
using NUnit.Framework;
using Steel.PlayTests.Support;
using Steel.Presentation.UI;
using Steel.Presentation.Vehicles.Haul;
using Steel.Presentation.World.Yard;
using Steel.Services;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Steel.PlayTests.Haul
{
    /// <summary>The loading post, the weighbridge and the truck's bed as built in the city.</summary>
    public sealed class TruckHaulSceneTests
    {
        [UnityTest] public IEnumerator BothPostsStandOnOpenGroundAndEveryTruckHasAnEmptyBed()
        {
            yield return OwnedFleet.LoadWealthyCity();
            var game = Object.FindObjectOfType<GameHost>().Game;
            foreach (SliceLocation post in new[] { game.YardPost, game.GatePost })
            {
                var at = new Vector3(post.X, 0, post.Z);
                Assert.IsFalse(Physics.CheckSphere(at + Vector3.up * 1.2f, .4f, Physics.DefaultRaycastLayers,
                    QueryTriggerInteraction.Ignore), "Something solid stands on the post at " + at);
                Assert.AreNotEqual("nothing", GroundProbe.Under(at + Vector3.up * 2), "No ground under the post at " + at);
            }
            var beds = Object.FindObjectsOfType<TruckLoadView>();
            CollectionAssert.AreEquivalent(new[] { FleetCatalog.Engkel, FleetCatalog.Tronton, FleetCatalog.Trailer },
                beds.Select(bed => bed.Truck).ToArray());
            yield return new WaitForSeconds(.6f);
            foreach (TruckLoadView bed in beds) Assert.IsFalse(bed.Load.gameObject.activeSelf, bed.Truck + " shows a load it does not carry");
            var works = Object.FindObjectOfType<YardWorks>();
            Assert.IsFalse(works.Loader.gameObject.activeSelf, "No loader works a yard that has not opened");
            Assert.IsTrue(works.Heaps.All(heap => !heap.gameObject.activeSelf));
            var caller = works.GetComponent<YardCaller>();
            Assert.IsFalse(caller.Collector.gameObject.activeSelf, "Nobody brings scrap to a yard that buys nothing");
            StringAssert.Contains("TIMBANGAN", caller.Readout.text);
        }

        [UnityTest] public IEnumerator TheWeighingPostOpensItsScreenAndRefusesWithoutATruck()
        {
            yield return OwnedFleet.LoadWealthyCity();
            var host = Object.FindObjectOfType<GameHost>(); var hud = Object.FindObjectOfType<CityHud>();
            OwnedFleet.PlacePlayer(host, host.Game.GatePost.X, host.Game.GatePost.Z); yield return null;
            Assert.AreEqual("gate", host.Game.Nearby.Id);
            hud.Act();
            Transform panel = hud.SafeRoot.Find("Pos truk"); Assert.IsTrue(panel.gameObject.activeSelf);
            double cash = host.Game.View.Cash;
            panel.GetComponentsInChildren<Button>().Single(b => b.name == "TIMBANG & BONGKAR").onClick.Invoke();
            Assert.AreEqual(cash, host.Game.View.Cash);
            StringAssert.Contains("Parkir truk", panel.GetComponentsInChildren<Text>().Last(t => t.color == UiFactory.Gold).text);
            panel.GetComponentsInChildren<Button>().Single(b => b.name == "TUTUP").onClick.Invoke();
            Assert.IsFalse(panel.gameObject.activeSelf);
        }
    }
}
