using System.Collections;
using System.Linq;
using NUnit.Framework;
using Steel.Presentation.UI;
using Steel.Presentation.World.Risk;
using Steel.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Steel.PlayTests.Risk
{
    /// <summary>The yard's trouble in the built city: the gang stands at the gate, the desk answers it, and hired guards stand watch.</summary>
    public sealed class SecurityDeskTests
    {
        [UnityTest] public IEnumerator AGangShowsAtTheGateTheDeskPaysItAndAHiredGuardStandsWatch()
        {
            IConfigSource source = new OverrideConfigSource(new ResourceConfigSource(), "City", "startCash", 900000000);
            source = new OverrideConfigSource(source, "Business", "minimumReputation", 1);
            source = new OverrideConfigSource(source, "Crew", "newcomerHours", 0);
            source = new OverrideConfigSource(new OverrideConfigSource(source, "Risk", "extortChance", 1), "Risk", "extortCap", 1);
            GameHost.Source = source;
            yield return SceneManager.LoadSceneAsync("City");
            GameHost.Source = null;
            yield return null;
            var host = Object.FindObjectOfType<GameHost>(); IGameService g = host.Game;
            var trouble = Object.FindObjectOfType<GateTrouble>();
            Assert.IsFalse(trouble.Thugs.Any(thug => thug.activeSelf), "Nobody waits at a yard that is not open");
            var pile = host.Layout.Piles[0]; var home = host.Layout.Home;
            g.SetPosition(pile.Position.X, pile.Position.Z); g.Classify(pile.Id, 0, false); g.PickUp(pile.Id);
            g.SetPosition(home.X, home.Z); g.Classify(null, 0, false); g.InspectBid(0); g.SellLocal();
            g.ApplyPermit(true); g.Lease(); g.Advance(24 * 600); g.OpenYard();
            for (int hour = 0; hour < 30 && !g.Risk.Demand; hour++) g.Advance(600);
            RiskView risk = g.Risk;
            Assert.IsTrue(risk.Demand, "No gang came to a rich, unguarded yard"); Assert.Greater(risk.Thugs, 0);
            yield return new WaitForSeconds(1.2f);
            Assert.AreEqual(Mathf.Min(risk.Thugs, trouble.Thugs.Length), trouble.Thugs.Count(thug => thug.activeSelf));
            foreach (GameObject thug in trouble.Thugs.Where(thug => thug.activeSelf))
                Assert.That(thug.transform.position.y, Is.InRange(-.1f, .3f), thug.name + " floats or sinks");

            Steel.PlayTests.Support.OwnedFleet.PlacePlayer(host, g.YardPost.X, g.YardPost.Z); yield return null;
            Assert.AreEqual("yardpost", g.Nearby.Id);
            var hud = Object.FindObjectOfType<CityHud>(); hud.Act();
            Button Key(string panel, string name) =>
                hud.SafeRoot.Find(panel).GetComponentsInChildren<Button>(true).Single(b => b.name == name);
            Key("Pos truk", "KEAMANAN").onClick.Invoke();
            Assert.IsTrue(hud.SafeRoot.Find("Pos keamanan").gameObject.activeSelf);
            double cash = g.View.Cash;
            Key("Pos keamanan", "Pos 4").onClick.Invoke();
            Assert.AreEqual(0, g.Risk.Guards[4], "Guards cannot be hired with the gang already there");
            Key("Pos keamanan", "Jawab 0").onClick.Invoke();
            Assert.IsFalse(g.Risk.Demand); Assert.AreEqual(cash - risk.DemandDue, g.View.Cash, 1e-6);
            Key("Pos keamanan", "Pos 4").onClick.Invoke();
            Assert.AreEqual(1, g.Risk.Guards[4]); Assert.AreEqual(cash - risk.DemandDue - risk.GuardCost[1], g.View.Cash, 1e-6);
            yield return new WaitForSeconds(1.2f);
            Assert.IsFalse(trouble.Thugs.Any(thug => thug.activeSelf), "Paid, they leave");
            Assert.AreEqual(1, trouble.Guards.Count(guard => guard.activeSelf));
            Key("Pos keamanan", "KEMBALI").onClick.Invoke(); Key("Pos truk", "TUTUP").onClick.Invoke();
        }
    }
}
