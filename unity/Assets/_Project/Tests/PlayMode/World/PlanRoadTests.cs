using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Steel.PlayTests.Support;
using Steel.Presentation.World;
using Steel.Services.Map;

namespace Steel.PlayTests.World
{
    /// <summary>What the big map promises a shared city: open carriageways everywhere, and plots waiting for players.</summary>
    public sealed class PlanRoadTests
    {
        private const float KerbRoom = 1, Low = .6f, High = 3.2f;   // from above a kerb to a truck's roof
        private const float EdgeRoom = 6;
        private const int Step = 3;

        [UnityTest]
        public IEnumerator NothingSolidStandsOnAnyPlannedStreet()
        {
            yield return SceneManager.LoadSceneAsync("City");
            yield return null;
            MapPlan plan = MapPlan.Load();
            var blockers = new List<string>();
            foreach (MapWay way in plan.ways.Where(way => way.IsRoad))
                for (int index = 1; index < way.xs.Length - 1; index += Step)
                {
                    Vector3 here = MapRoute.Lane(way, index, 0), ahead = MapRoute.Lane(way, index + 1, 0) - here;
                    if (!plan.bounds.Holds(here.x, here.z, -EdgeRoom)) continue;   // streets end at the wall around the world
                    var half = new Vector3(way.width / 2 - KerbRoom, (High - Low) / 2, 1);
                    foreach (Collider hit in Physics.OverlapBox(here + Vector3.up * (High + Low) / 2, half, Quaternion.LookRotation(ahead),
                        Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                        blockers.Add(way.name + " at " + here.ToString("F0") + ": " + hit.name);
                }
            Assert.IsEmpty(blockers, "Vehicles cannot pass: " + string.Join("; ", blockers.Distinct().Take(60)));
        }

        [UnityTest]
        public IEnumerator EveryPlannedStreetIsPavedFromKerbToKerb()
        {
            yield return SceneManager.LoadSceneAsync("City");
            yield return null;
            MapPlan plan = MapPlan.Load();
            var holes = new List<string>();
            foreach (MapWay way in plan.ways.Where(way => way.IsRoad))
                for (int index = 1; index < way.xs.Length - 1; index++)
                    foreach (float lane in new[] { -way.width / 2 + KerbRoom, -way.width / 4, way.width / 4, way.width / 2 - KerbRoom })
                    {
                        Vector3 spot = MapRoute.Lane(way, index, lane);
                        if (!plan.bounds.Holds(spot.x, spot.z, -EdgeRoom) || plan.core.Holds(spot.x, spot.z, EdgeRoom)) continue;
                        if (GroundProbe.Gap(spot) > 0) holes.Add(way.name + " at " + spot.ToString("F0"));   // the terrain lies below zero
                    }
            Assert.IsEmpty(holes, holes.Count + " holes in the streets: " + string.Join("; ", holes.Distinct().Take(40)));
        }

        [UnityTest]
        public IEnumerator TheCityKeepsPlotsForPlayers()
        {
            yield return SceneManager.LoadSceneAsync("City");
            yield return null;
            PlayerPlot[] plots = Object.FindObjectsOfType<PlayerPlot>();
            Assert.GreaterOrEqual(plots.Count(plot => plot.Kind == PlayerPlot.Lapak), 6, "Lapak plots for players");
            Assert.GreaterOrEqual(plots.Count(plot => plot.Kind == PlayerPlot.Factory), 1, "Factory plots for players");
            Assert.AreEqual(plots.Length, plots.Select(plot => plot.Kind + plot.Number).Distinct().Count(), "Every plot has its own number");
        }
    }
}
