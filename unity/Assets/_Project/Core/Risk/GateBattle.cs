using System;
using System.Collections.Generic;
using System.Linq;

namespace Steel.Core.Risk
{
    /// <summary>The gate auto-battler (PRD appendix D): three waves against the guards on the two front rows, one round a second
    /// for at most sixty. No dice are thrown, so the result is the same watched, skipped or offline.</summary>
    public sealed partial class GateBattle
    {
        private readonly RiskRules rules;
        private readonly int[] guards;
        private readonly double[] health;
        private List<int> foes;
        private List<double> foeHealth;
        private double gate;

        private GateBattle(int[] guards, RiskRules rules)
        {
            this.rules = rules; this.guards = guards; gate = rules.Get("gateHp");
            health = guards.Select(kind => kind > 0 ? rules.GuardHp(kind) : 0).ToArray();
        }

        private bool Standing(int kind) => Enumerable.Range(0, RiskRules.Posts).Any(cell => guards[cell] == kind && health[cell] > 0);

        public static BattleResult Fight(int[] guards, int youths, int caste, RiskRules rules)
        {
            if (guards == null || guards.Length != RiskRules.Cells) throw new ArgumentException("Grid gerbang harus 5x4.");
            var battle = new GateBattle(guards, rules);
            int rounds = 0, most = (int)rules.Get("battleRounds"), wave = 0;
            List<int>[] waves = GangWaves.For(youths, caste);
            for (; wave < waves.Length && rounds < most; wave++)
            {
                battle.Arrive(waves[wave]);
                bool seen = battle.Standing(RiskRules.Cctv);   // watched coming: the guards strike once before the wave does
                while (battle.foes.Count > 0 && battle.gate > 0 && rounds < most)
                {
                    rounds++;
                    battle.GuardsStrike();
                    if (seen) seen = false; else battle.GangStrikes();
                }
                if (battle.foes.Count > 0) break;   // the gate fell or the minute ran out with this wave still there
            }
            bool[] down = guards.Select((kind, cell) => kind > 0 && battle.health[cell] <= 0).ToArray();
            return new BattleResult(battle.gate > 0, rounds, wave, Math.Max(0, battle.gate), down);
        }

        /// <summary>A wave walks up. With a negotiator standing, one youth in five (rounded up) turns back.</summary>
        private void Arrive(List<int> wave)
        {
            foes = new List<int>(wave);
            if (Standing(RiskRules.Talker))
                for (int leave = (int)Math.Ceiling(foes.Count(kind => kind == RiskRules.Youth) * rules.Get("talkerLeave")); leave > 0; leave--)
                    foes.Remove(RiskRules.Youth);
            foeHealth = foes.Select(kind => rules.FoeHp(kind)).ToList();
        }
    }
}
