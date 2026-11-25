using System.Linq;
using NUnit.Framework;
using Steel.Core.Risk;
using Steel.Services;

namespace Steel.Tests.Risk
{
    /// <summary>The gate auto-battler (PRD appendix D): three waves against the two front rows, sixty rounds, the same result every time.</summary>
    public sealed class GateBattleTests
    {
        private readonly RiskRules rules = new RiskRules(new ResourceConfigSource().Read("Risk"));

        /// <summary>Row 0 faces the street; each row is five kinds across.</summary>
        private static int[] Grid(int[] front, int[] second = null)
        {
            var grid = new int[RiskRules.Cells];
            front.CopyTo(grid, 0); second?.CopyTo(grid, RiskRules.Columns);
            return grid;
        }

        private static int[] Row(int kind) => Enumerable.Repeat(kind, RiskRules.Columns).ToArray();

        [Test]
        public void TheUnitsAndTheGangCarryTheNumbersOfAppendixD()
        {
            double[] hp = { 120, 80, 50, 40, 40, 150 }, hit = { 8, 18, 14, 0, 0, 22 };
            for (int kind = 1; kind <= RiskRules.Kinds; kind++)
            { Assert.AreEqual(hp[kind - 1], rules.GuardHp(kind)); Assert.AreEqual(hit[kind - 1], rules.GuardHit(kind)); }
            Assert.AreEqual(50000000, rules.GuardCost(RiskRules.Escort));
            Assert.AreEqual(new[] { 60d, 140, 40, 220 }, Enumerable.Range(1, 4).Select(rules.FoeHp).ToArray());
            Assert.AreEqual(new[] { 10d, 20, 5, 25 }, Enumerable.Range(1, 4).Select(rules.FoeHit).ToArray());
        }

        [Test]
        public void ThreeWavesComeChampionsFromTheSecondAndALeaderOnlyAtKastaThree()
        {
            var waves = GangWaves.For(9, 2);
            Assert.AreEqual(3, waves.Length); Assert.IsTrue(waves[0].All(kind => kind == RiskRules.Youth));
            Assert.AreEqual(9, waves.Sum(wave => wave.Count(kind => kind == RiskRules.Youth)));
            Assert.AreEqual(2, waves[1].Count(kind => kind == RiskRules.Champion)); Assert.AreEqual(RiskRules.Agitator, waves[1].Last());
            Assert.IsFalse(waves.Any(wave => wave.Contains(RiskRules.Leader)));
            Assert.AreEqual(1, GangWaves.For(9, 3)[2].Count(kind => kind == RiskRules.Leader));
        }

        [Test]
        public void AnEmptyGateFallsInTheFirstWaveAndAFullWatchBeatsAllThree()
        {
            BattleResult open = GateBattle.Fight(new int[RiskRules.Cells], 6, 2, rules);
            Assert.IsFalse(open.Held); Assert.AreEqual(0, open.Waves); Assert.AreEqual(0, open.GuardsDown);
            BattleResult full = GateBattle.Fight(Grid(Row(RiskRules.Tank), Row(RiskRules.Baton)), 9, 2, rules);
            Assert.IsTrue(full.Held); Assert.AreEqual(3, full.Waves); Assert.LessOrEqual(full.Rounds, 60);
        }

        [Test]
        public void AnOpenPostLosesTheGateAFullLineHoldsAndATankHoldsTwoMen()
        {
            int[] gap = Row(RiskRules.Baton); gap[2] = 0;
            Assert.IsTrue(GateBattle.Fight(Grid(Row(RiskRules.Baton)), 9, 2, rules).Held);
            Assert.IsFalse(GateBattle.Fight(Grid(gap), 9, 2, rules).Held, "The gang rushes the open middle post");
            // one round, six men, one guard in the middle: a baton holds one and five hit the gate, a tank holds two
            var round = new RiskRules(TestConfig.With("Risk", "battleRounds", 1));
            int[] tank = new int[RiskRules.Columns], baton = new int[RiskRules.Columns];
            tank[2] = RiskRules.Tank; baton[2] = RiskRules.Baton;
            Assert.AreEqual(150, GateBattle.Fight(Grid(baton), 18, 2, round).GateLeft);
            Assert.AreEqual(160, GateBattle.Fight(Grid(tank), 18, 2, round).GateLeft);
        }

        [Test]
        public void TheDogTheCctvAndTheNegotiatorEachSaveTheGateAndAGuard()
        {
            BattleResult plain = GateBattle.Fight(Grid(Row(RiskRules.Baton)), 9, 2, rules);
            foreach (int support in new[] { RiskRules.Dog, RiskRules.Cctv, RiskRules.Talker })
            {
                int[] second = new int[RiskRules.Columns]; second[2] = support;
                BattleResult helped = GateBattle.Fight(Grid(Row(RiskRules.Baton), second), 9, 2, rules);
                Assert.Greater(helped.GateLeft, plain.GateLeft, "Unit " + support);
                Assert.Less(helped.GuardsDown, plain.GuardsDown, "Unit " + support);
            }
        }

        [Test]
        public void TheSameGridAndGangAlwaysEndTheSameWay()
        {
            int[] grid = Grid(new[] { 1, 2, 5, 2, 1 }, new[] { 3, 0, 4, 0, 3 });
            BattleResult first = GateBattle.Fight(grid, 12, 3, rules), again = GateBattle.Fight(grid, 12, 3, rules);
            Assert.AreEqual(first.Held, again.Held); Assert.AreEqual(first.Rounds, again.Rounds);
            Assert.AreEqual(first.GateLeft, again.GateLeft); Assert.AreEqual(first.Down, again.Down);
        }
    }
}
