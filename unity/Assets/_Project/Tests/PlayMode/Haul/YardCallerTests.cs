using System.Collections;
using NUnit.Framework;
using Steel.Presentation.Vehicles.Haul;
using Steel.Presentation.World.Yard;
using Steel.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Steel.PlayTests.Haul
{
    /// <summary>The player's yard seen working: stock on the ground, the loader out, and a collector at the scale after a purchase.</summary>
    public sealed class YardCallerTests
    {
        [UnityTest] public IEnumerator ACollectorPushesHisCartToTheScaleWhenTheYardBuysAndTheHeapAppears()
        {
            IConfigSource source = new OverrideConfigSource(new ResourceConfigSource(), "City", "startCash", 900000000);
            GameHost.Source = new OverrideConfigSource(source, "Business", "minimumReputation", 1);
            yield return SceneManager.LoadSceneAsync("City");
            GameHost.Source = null;
            yield return null;
            var host = Object.FindObjectOfType<GameHost>(); IGameService g = host.Game;
            var works = Object.FindObjectOfType<YardWorks>(); var caller = works.GetComponent<YardCaller>();
            var pile = host.Layout.Piles[0]; var home = host.Layout.Home;
            g.SetPosition(pile.Position.X, pile.Position.Z); g.Classify(pile.Id, 0, false); g.PickUp(pile.Id);
            g.SetPosition(home.X, home.Z); g.Classify(null, 0, false); g.InspectBid(0); g.SellLocal();
            g.ApplyPermit(true); g.Lease(); g.Advance(24 * 600); g.OpenYard();
            g.SetBid(0, 6000); g.Advance(4 * 600);
            Assert.Greater(g.LastVisit.Serial, 0); Assert.Greater(g.YardInfo(0).Stock, 0);
            Time.timeScale = 6;
            try
            {
                float limit = Time.realtimeSinceStartup + 60;
                while (!caller.Collector.gameObject.activeSelf && Time.realtimeSinceStartup < limit) yield return null;
                Assert.IsTrue(caller.Collector.gameObject.activeSelf, "No collector came for the lot the yard bought");
                Assert.IsTrue(works.Loader.gameObject.activeSelf); Assert.IsTrue(works.Heaps[0].gameObject.activeSelf);
                Assert.IsFalse(works.Heaps[1].gameObject.activeSelf, "Only the grade that was bought has a heap");
                while (!caller.Readout.text.Contains("kg") && Time.realtimeSinceStartup < limit) yield return null;
                StringAssert.Contains(g.LastVisit.Grade, caller.Readout.text);
                Assert.Less(Vector3.Distance(caller.Collector.localPosition, caller.Scale), .05f, "He reads the scale standing at it");
                while (caller.Collector.gameObject.activeSelf && Time.realtimeSinceStartup < limit) yield return null;
                Assert.IsFalse(caller.Collector.gameObject.activeSelf, "He leaves once he is paid");
            }
            finally { Time.timeScale = 1; }
        }
    }
}
