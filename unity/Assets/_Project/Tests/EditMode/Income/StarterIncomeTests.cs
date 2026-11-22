using NUnit.Framework;
using Steel.Services;
using Steel.Tests.Support;

namespace Steel.Tests.Income
{
    /// <summary>"Kalau besi di pinggir jalan habis, dapat uang dari mana?" played from the starting balance.</summary>
    public sealed class StarterIncomeTests
    {
        private Scavenger player;
        private LocalGameService g;
        [SetUp] public void Start() { player = new Scavenger(); g = player.Game; }

        [Test] public void EarningContinuesAfterEveryStreetSiteIsEmptied()
        {
            Assert.AreEqual(500000, g.View.Cash); Assert.AreEqual(50, g.View.CapacityKg);
            StringAssert.Contains("Periksa tumpukan, lalu pungut. Sisa 10 tumpukan.", player.Hint);
            player.ClearTheStreet();
            var bare = g.Income; double cash = g.View.Cash;
            Assert.Zero(bare.StreetSites); Assert.Zero(g.View.CarriedKg); Assert.Greater(cash, 500000);
            Assert.AreEqual(20, bare.RouteKg); Assert.AreEqual(30, bare.RouteMinutes);
            Assert.That(bare.RestockMinutes, Is.GreaterThan(0).And.LessThanOrEqualTo(5));
            StringAssert.Contains("ambil hadiah setoran pertama", player.Hint);
            StringAssert.Contains("BUKA ARMADA, pesan rute gratis 20 kg (30 menit).", player.Earning);
            StringAssert.Contains("Warga menaruh tumpukan baru", player.Earning);

            player.Home(); Assert.IsTrue(g.Cart.Held);
            g.BookRoute(g.Route("cart", 0, false));
            Assert.IsFalse(g.Cart.Held, "Booking parks the empty cart instead of refusing");
            Assert.AreEqual(cash, g.View.Cash, "The cart round costs nothing");
            StringAssert.Contains("Kiriman rute tiba di lapak 30 menit lagi.", player.Earning);

            player.Wait(g.Income.ArrivalMinutes);
            Assert.AreEqual(20, g.Income.DepotKg); Assert.Zero(g.Income.CartAwayMinutes);
            Assert.GreaterOrEqual(g.Income.StreetSites, 6, "Residents refilled a site every five minutes meanwhile");
            StringAssert.Contains("Kiriman 20 kg sudah tiba. Buka armada, ketuk AMBIL KIRIMAN.", player.Earning);
            player.Home(); player.TakeCart(); Assert.AreEqual(20, g.CollectRoute()); player.SellAll();
            double afterRound = g.View.Cash; Assert.Greater(afterRound, cash);

            StringAssert.StartsWith("Gang ", player.Earning); StringAssert.Contains("Sisa ", player.Earning);
            player.ClearTheStreet();
            Assert.Greater(g.View.Cash, afterRound);
            Assert.IsFalse(g.View.GrantCollected, "The dealer grant was never needed to keep earning");
        }

        [TestCase(false, 5, TestName = "ADayOfStreetPickingAloneNeverWaitsLongerThanOneRefill")]
        [TestCase(true, 5, TestName = "ADayOfFollowingEveryHintWithTheCartAwayNeverWaitsLongerThanOneRefill")]
        public void ADayWithoutGrant(bool rounds, double longestWait)
        {
            player.ClearTheStreet();
            double day = g.GameHours + 24, start = g.View.Cash, last = start;
            while (g.GameHours < day)
            {
                player.FollowHint(rounds);
                Assert.GreaterOrEqual(g.View.Cash, last, "Scavenging never costs money"); last = g.View.Cash;
            }
            Assert.LessOrEqual(player.LongestWait, longestWait);
            Assert.Greater(g.View.Cash - start, 1000000, "A game day after the street ran out still pays");
            Assert.IsFalse(g.View.GrantCollected); Assert.IsFalse(g.VehicleOf(FleetCatalog.Triseda).Owned);
        }
    }
}
