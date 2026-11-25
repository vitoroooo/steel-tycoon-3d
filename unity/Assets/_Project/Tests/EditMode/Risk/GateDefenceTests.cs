using System;
using NUnit.Framework;
using Steel.Core.Risk;

namespace Steel.Tests.Risk
{
    /// <summary>Holding the gate (PRD appendix D.1): won, two quiet game months and +5 reputation; lost, the security money plus the gate.</summary>
    public sealed class GateDefenceTests : RiskFixture
    {
        [Test]
        public void GuardsPostedBeforehandHoldTheGateForTwoQuietMonthsAndFiveReputation()
        {
            Open(Rules(Certain));
            double cash = Game.Cash;
            for (int column = 0; column < RiskRules.Columns; column++)
            { Game.PostGuard(column, RiskRules.Tank); Game.PostGuard(RiskRules.Columns + column, RiskRules.Baton); }
            Assert.AreEqual(cash - 5 * (6000000 + 4000000), Game.Cash, 1e-6);
            Assert.Throws<InvalidOperationException>(() => Game.PostGuard(RiskRules.Posts, RiskRules.Tank), "Only the two front rows");
            Assert.Throws<InvalidOperationException>(() => Game.PostGuard(0, RiskRules.Escort), "The escort comes only for a gang");
            Game.AdvanceRisk(25); Assert.IsTrue(Game.Risk.demand);
            Assert.Throws<InvalidOperationException>(() => Game.PostGuard(0, 0), "The watch cannot change with the gang there");
            cash = Game.Cash;
            BattleResult result = Game.DefendGate(26);
            Assert.IsTrue(result.Held); Assert.AreEqual(cash, Game.Cash, "A held gate costs nothing");
            Assert.AreEqual(26 + 1440, Game.Risk.quietUntil); Assert.AreEqual(65, Game.Yard.reputation);
        }

        [Test]
        public void AnUnguardedGateFallsAndCostsTheSecurityMoneyPlusTheGate()
        {
            Arrive(); double cash = Game.Cash;
            Assert.IsFalse(Game.DefendGate(26).Held);
            Assert.AreEqual(cash - 10000000 - 50000000, Game.Cash, 1e-6); StringAssert.Contains("jebol", Game.Risk.note);
            Assert.AreEqual(26 + 720, Game.Risk.quietUntil);
        }

        [Test]
        public void AnEscortTeamIsHiredForOneGangOnlyAndKnockedOutGuardsAreBackAfterAGameDay()
        {
            Arrive(); double cash = Game.Cash;
            Game.PostGuard(2, RiskRules.Escort);
            Assert.AreEqual(cash - 50000000, Game.Cash, 1e-6);
            BattleResult result = Game.DefendGate(26);
            Assert.AreEqual(0, Game.Risk.guards[2], "They leave when it is over");
            Assert.IsFalse(result.Held, "One team cannot cover five posts");

            Open(Rules(Certain)); Game.PostGuard(2, RiskRules.Baton);
            Game.AdvanceRisk(25); Game.DefendGate(26);
            Assert.AreEqual(0, Game.Risk.OnDuty(40)[2], "Out cold"); Assert.AreEqual(RiskRules.Baton, Game.Risk.OnDuty(50)[2]);
        }
    }
}
