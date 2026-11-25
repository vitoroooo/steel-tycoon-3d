using System.Linq;
using NUnit.Framework;
using Steel.Services;
using Steel.Services.Map;

namespace Steel.Tests.Map
{
    /// <summary>The plan and the vehicle configs are written apart; these keep them pointing at the same ground.</summary>
    public sealed class MapSiteTests
    {
        [Test] public void EveryHeavyTruckIsParkedOnTheTruckDealersLot()
        {
            MapSite dealer = MapPlan.Load().sites.Single(site => site.kind == MapSite.TruckDealer);
            var game = new LocalGameService(new ResourceConfigSource());
            foreach (string id in FleetCatalog.Heavy)
            {
                VehicleView truck = game.VehicleOf(id);
                Assert.IsTrue(dealer.Holds(truck.X, truck.Z), id + " at " + truck.X + ", " + truck.Z + " stands off " + dealer.name);
            }
        }

        [Test] public void SitesLieInsideTheCityAndOutsideTheOldTown()
        {
            MapPlan plan = MapPlan.Load();
            foreach (MapSite site in plan.sites)
            {
                Assert.IsTrue(plan.bounds.Holds(site.x, site.z), site.name + " is off the map");
                Assert.IsFalse(plan.core.Holds(site.x, site.z), site.name + " is inside the hand-built old town");
            }
        }

        [Test] public void ThePlanKeepsItsStreetClassesForHeavyTraffic()
        {
            MapPlan plan = MapPlan.Load();
            Assert.GreaterOrEqual(plan.ways.Single(way => way.kind == MapPlan.Ring).width, 18, "Trailers need the ring road");
            Assert.IsTrue(plan.ways.Where(way => way.kind == MapPlan.Artery).All(way => way.width >= 14), "Trontons need the arteries");
            Assert.IsTrue(plan.ways.Where(way => way.kind == MapPlan.Collector).All(way => way.width >= 9), "Engkels need the collectors");
            Assert.IsFalse(plan.places.Any(place => place.name.ToLowerInvariant().Contains("pelabuhan")), "Sukabaja has no port");
        }
    }
}
