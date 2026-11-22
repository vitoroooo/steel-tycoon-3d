using NUnit.Framework;
using Steel.Core.Risk;
using Steel.Services;

namespace Steel.Tests.Risk
{
    /// <summary>The gate auto-battler on its 5x4 grid (PRD 6.3): four lanes fought apart, the same result every time.</summary>
    public sealed class GateBattleTests
    {
        private readonly RiskRules rules = new RiskRules(new ResourceConfigSource().Read("Risk"));

        private static int[] Grid(params (int lane, int column, int kind)[] posts)
        {
            var grid = new int[RiskRules.Cells];
            foreach (var post in posts) grid[post.lane * RiskRules.Columns + post.column] = post.kind;
            return grid;
        }

        [Test]
        public void AnEmptyGateLetsEveryThugThrough()
        {
            BattleResult result = GateBattle.Fight(new int[RiskRules.Cells], 6, rules);
            Assert.IsFalse(result.Held); Assert.AreEqual(6, result.Breached); Assert.AreEqual(0, result.GuardsDown);
        }

        [Test]
        public void AGuardInEveryLaneHoldsFourThugsButAnOpenLaneDoesNot()
        {
            Assert.IsTrue(GateBattle.Fight(Grid((0, 4, 2), (1, 4, 2), (2, 4, 2), (3, 4, 2)), 4, rules).Held);
            BattleResult gap = GateBattle.Fight(Grid((0, 4, 2), (1, 4, 2), (2, 4, 2)), 4, rules);
            Assert.AreEqual(1, gap.Breached, "The fourth lane has a thug and nobody to stop him");
        }

        [Test]
        public void AShieldInFrontOfABatonHoldsWhatABatonAloneCannot()
        {
            int[] alone = Grid((0, 4, 2), (1, 4, 2), (2, 4, 2), (3, 4, 2));
            int[] backed = Grid((0, 4, 1), (0, 3, 2), (1, 4, 1), (1, 3, 2), (2, 4, 1), (2, 3, 2), (3, 4, 1), (3, 3, 2));
            Assert.IsFalse(GateBattle.Fight(alone, 8, rules).Held, "Two thugs a lane wear one baton down");
            BattleResult result = GateBattle.Fight(backed, 8, rules);
            Assert.IsTrue(result.Held); Assert.AreEqual(0, result.GuardsDown, "The shield takes the blows and stands");
        }

        [Test]
        public void ACalmerHalvesTheBlowsSoTheSameLineLastsLonger()
        {
            int[] plain = Grid((0, 4, 2)), calmed = Grid((0, 4, 2), (0, 0, 3));
            Assert.AreEqual(1, GateBattle.Fight(plain, 5, rules).GuardsDown);   // lane 0 gets two of five
            BattleResult result = GateBattle.Fight(calmed, 5, rules);
            Assert.AreEqual(0, result.GuardsDown); Assert.AreEqual(3, result.Breached, "Only the unguarded lanes are lost");
        }

        [Test]
        public void TheSameGridAndGangAlwaysEndTheSameWay()
        {
            int[] grid = Grid((0, 4, 1), (1, 2, 2), (2, 4, 3), (3, 1, 2));
            BattleResult first = GateBattle.Fight(grid, 9, rules), again = GateBattle.Fight(grid, 9, rules);
            Assert.AreEqual(first.Breached, again.Breached); Assert.AreEqual(first.GuardsDown, again.GuardsDown);
            Assert.AreEqual(first.Rounds, again.Rounds);
        }
    }
}
