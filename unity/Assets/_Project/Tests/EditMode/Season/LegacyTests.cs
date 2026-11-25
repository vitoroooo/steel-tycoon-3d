using System;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Season
{
    /// <summary>Bekal warisan (PRD 11.3): what each legacy changes in a new career, and that it is taken once.</summary>
    public sealed class LegacyTests
    {
        private static readonly IConfigSource Config = new ResourceConfigSource();
        private static LocalGameService Start(params string[] picks)
        {
            var g = new LocalGameService(Config);
            if (picks.Length > 0) g.StartWithLegacies(picks);
            return g;
        }

        [Test] public void ANewcomerStartsWithoutAnyLegacy()
        {
            var g = Start();
            Assert.IsEmpty(g.Legacies); Assert.IsTrue(g.LegacyOpen);
            Assert.IsFalse(g.VehicleOf(FleetCatalog.Triseda).Owned);
        }

        [Test] public void TheTrisedaReputationAndCompanyLegaciesAreThereFromTheFirstMinute()
        {
            var plain = Start(); var g = Start("triseda", "reputasi", "pt");
            Assert.IsTrue(g.VehicleOf(FleetCatalog.Triseda).Owned);
            Assert.AreEqual(plain.View.Cash, g.View.Cash, "A legacy never gives money");
            Assert.AreEqual(15, g.YardInfo(0).Rep); Assert.Less(plain.YardInfo(0).Rep, 15);
            Assert.Throws<InvalidOperationException>(() => g.ApplyPermit(true), "The company is already registered");
            Assert.IsFalse(g.LegacyOpen);
        }

        [Test] public void TheSortingLegacyShortensARoundByATenthAndTheQueueLegacyAddsTwoGameHours()
        {
            var plain = Start(); var g = Start("sortir", "antrean");
            Assert.AreEqual(plain.Route("cart", 0, false).Minutes * 0.9, g.Route("cart", 0, false).Minutes, 1e-9);
            Assert.AreEqual(plain.CrewInfo().OfflineHours + 2 * 600 / 3600d, g.CrewInfo().OfflineHours, 1e-9);
            Assert.IsFalse(g.VehicleOf(FleetCatalog.Triseda).Owned);
        }

        [Test] public void LegaciesAreTakenOnceAtMostThreeAndOnlyKnownOnes()
        {
            Assert.Throws<InvalidOperationException>(() => Start("triseda", "reputasi", "pt", "sortir"));
            Assert.Throws<InvalidOperationException>(() => Start("triseda", "triseda"));
            Assert.Throws<InvalidOperationException>(() => Start("uang"));
            var g = Start("reputasi");
            Assert.Throws<InvalidOperationException>(() => g.StartWithLegacies(new[] { "triseda" }));
        }

        [Test] public void ACareerThatHasAlreadySoldCannotTakeLegacies()
        {
            var city = new CityLayout(Config); var g = Start();
            var p = city.Piles[0]; g.SetPosition(p.Position.X, p.Position.Z);
            g.Classify(p.Id, 0, false); g.PickUp(p.Id);
            g.SetPosition(city.Home.X, city.Home.Z); g.Classify(null, 0, false); g.InspectBid(0); g.SellLocal();
            Assert.IsFalse(g.LegacyOpen);
            Assert.Throws<InvalidOperationException>(() => g.StartWithLegacies(new[] { "triseda" }));
        }

        [Test] public void LegaciesSurviveSaveAndLoad()
        {
            var g = Start("triseda", "sortir", "antrean");
            var plain = Start();
            g = LocalGameService.Resume(Config, g.Save(1), 1, out _);
            CollectionAssert.AreEqual(new[] { "triseda", "sortir", "antrean" }, g.Legacies);
            Assert.IsTrue(g.VehicleOf(FleetCatalog.Triseda).Owned);
            Assert.Less(g.Route("cart", 0, false).Minutes, plain.Route("cart", 0, false).Minutes);
            Assert.IsFalse(g.LegacyOpen);
        }
    }
}
