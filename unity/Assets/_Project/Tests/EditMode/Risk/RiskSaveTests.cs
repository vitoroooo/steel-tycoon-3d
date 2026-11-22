using System;
using NUnit.Framework;
using Steel.Core.Risk;
using Steel.Services;

namespace Steel.Tests.Risk
{
    /// <summary>Trouble survives a reload: the gang still waits, guards stay posted, heat stays, and nothing can be rolled or paid twice.</summary>
    public sealed class RiskSaveTests : RiskFixture
    {
        private static LocalGameService Yard(IConfigSource src)
        {
            var city = new CityLayout(src); var g = new LocalGameService(src);
            var p = city.Piles[0]; g.SetPosition(p.Position.X, p.Position.Z);
            g.Classify(p.Id, 0, false); g.PickUp(p.Id);
            g.SetPosition(city.Home.X, city.Home.Z); g.Classify(null, 0, false); g.InspectBid(0); g.SellLocal();
            g.ApplyPermit(true); g.Lease(); g.Advance(24 * 600); g.OpenYard();
            g.SetPosition(g.YardPost.X, g.YardPost.Z);
            return g;
        }

        private static IConfigSource Source(params (string key, double value)[] risk)
        {
            IConfigSource src = new OverrideConfigSource(new ResourceConfigSource(), "City", "startCash", 900000000);
            src = new OverrideConfigSource(src, "Business", "minimumReputation", 1);
            src = new OverrideConfigSource(src, "Crew", "newcomerHours", 0);
            foreach (var change in risk) src = new OverrideConfigSource(src, "Risk", change.key, change.value);
            return src;
        }

        [Test]
        public void AGangAtTheGateAndThePostedGuardsComeBackAfterAReload()
        {
            IConfigSource src = Source(("extortChance", 0));
            LocalGameService g = Yard(src);
            g.PostGuard(4, 1); g.PostGuard(9, 3); g.ToggleAutoDefend();
            src = Source(("extortChance", 1), ("extortCap", 1));
            g = LocalGameService.Resume(src, g.Save(1), 1, out _);
            Assert.AreEqual(1, g.Risk.Guards[4]); Assert.AreEqual(3, g.Risk.Guards[9]); Assert.IsTrue(g.Risk.AutoDefend);
            for (int hour = 0; hour < 30 && !g.Risk.Demand; hour++) g.Advance(600);
            RiskView before = g.Risk; Assert.IsTrue(before.Demand);
            g = LocalGameService.Resume(src, g.Save(2), 2, out _);
            Assert.IsTrue(g.Risk.Demand); Assert.AreEqual(before.DemandDue, g.Risk.DemandDue); Assert.AreEqual(before.Thugs, g.Risk.Thugs);
            g.SetPosition(g.YardPost.X, g.YardPost.Z);
            double cash = g.View.Cash; g.AnswerGang(0);
            g = LocalGameService.Resume(src, g.Save(3), 3, out _);
            Assert.IsFalse(g.Risk.Demand); Assert.AreEqual(cash - before.DemandDue, g.View.Cash, 1e-6);
            Assert.Throws<InvalidOperationException>(() => g.AnswerGang(0), "Paid once");
        }

        [Test]
        public void HeatAndAnAnsweredOfferComeBackSoAReloadCannotRollAgain()
        {
            IConfigSource src = Source(("offerChance", 1), ("raidBase", 1e-9), ("raidPerHeat", 1e-9), ("extortChance", 0));
            LocalGameService g = Yard(src);
            Assert.IsTrue(g.Risk.Offer); g.AnswerContraband(0);
            double cash = g.View.Cash;
            g = LocalGameService.Resume(src, g.Save(1), 1, out _);
            Assert.AreEqual(15, g.Risk.Heat); Assert.IsFalse(g.Risk.Offer); Assert.AreEqual(cash, g.View.Cash, 1e-6);
            Assert.Throws<InvalidOperationException>(() => g.AnswerContraband(0));
        }

        [Test]
        public void ARaidSealsAKastaThreeOperationForTwoGameDays()
        {
            Open(Rules(("offerChance", 1), ("raidBase", 1)));
            Game.Mill.filed = Game.Mill.opened = true;
            Assert.IsTrue(Game.AcceptContraband(Offer(30), 30));
            Assert.AreEqual(78, Game.Risk.seizedUntil);
            Assert.AreEqual("Operasional disita setelah razia.", Game.CrewStopReason(40, 100));
            Assert.AreNotEqual("Operasional disita setelah razia.", Game.CrewStopReason(78, 100));
        }
    }
}
