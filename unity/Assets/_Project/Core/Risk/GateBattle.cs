using System;
using System.Collections.Generic;

namespace Steel.Core.Risk
{
    /// <summary>How a fight at the gate ended.</summary>
    public readonly struct BattleResult
    {
        public readonly int Breached, GuardsDown, Rounds;
        public bool Held => Breached == 0;
        public BattleResult(int breached, int guardsDown, int rounds) { Breached = breached; GuardsDown = guardsDown; Rounds = rounds; }
    }

    /// <summary>The gate auto-battler (PRD 6.3): the gang splits evenly over four lanes and each lane is fought on its own.
    /// Every round all standing guards hit the gang and the gang hits the guard nearest the street; a calmer halves its blows.</summary>
    public static class GateBattle
    {
        private const int MostRounds = 400;

        public static BattleResult Fight(int[] guards, int thugs, RiskRules rules)
        {
            if (guards == null || guards.Length != RiskRules.Cells) throw new ArgumentException("Grid gerbang harus 5x4.");
            int breached = 0, down = 0, rounds = 0;
            for (int lane = 0; lane < RiskRules.Lanes; lane++)
            {
                int gang = thugs / RiskRules.Lanes + (lane < thugs % RiskRules.Lanes ? 1 : 0);
                var line = new List<int>();
                for (int column = RiskRules.Columns - 1; column >= 0; column--)
                    if (guards[lane * RiskRules.Columns + column] > 0) line.Add(guards[lane * RiskRules.Columns + column]);
                breached += Lane(line, gang, rules, ref down, ref rounds);
            }
            return new BattleResult(breached, down, rounds);
        }

        /// <summary>Thugs of this lane that got past its guards.</summary>
        private static int Lane(List<int> line, int gang, RiskRules rules, ref int down, ref int rounds)
        {
            double thugHp = rules.Get("thugHp"), pool = gang * thugHp;
            var health = new double[line.Count];
            for (int i = 0; i < health.Length; i++) health[i] = rules.GuardHp(line[i]);
            int front = 0;
            for (int round = 0; round < MostRounds && pool > 1e-9 && front < line.Count; round++)
            {
                int standing = (int)Math.Ceiling(pool / thugHp);
                double hit = 0; bool calm = false;
                for (int i = front; i < line.Count; i++) { hit += rules.GuardHit(line[i]); calm |= line[i] == RiskRules.Calmer; }
                double blows = standing * rules.Get("thugHit") * (calm ? rules.Get("tamerCalm") : 1);
                pool -= hit;
                while (blows > 0 && front < line.Count)
                {
                    double taken = Math.Min(blows, health[front]);
                    health[front] -= taken; blows -= taken;
                    if (health[front] <= 1e-9) { front++; down++; }
                }
                rounds = Math.Max(rounds, round + 1);
            }
            return pool > 1e-9 ? (int)Math.Ceiling(pool / thugHp) : 0;
        }
    }
}
