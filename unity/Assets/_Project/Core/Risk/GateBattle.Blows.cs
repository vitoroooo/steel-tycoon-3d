using System;
using System.Linq;

namespace Steel.Core.Risk
{
    /// <summary>One round of blows at the gate, first the guards', then the gang's.</summary>
    public sealed partial class GateBattle
    {
        /// <summary>Every standing guard hits the nearest of the gang; the dog goes for the back, and a baton that fells a man
        /// carries on into the next. A CCTV operator makes everyone else's blows a tenth better.</summary>
        private void GuardsStrike()
        {
            double aim = Standing(RiskRules.Cctv) ? rules.Get("cctvAim") : 1;
            for (int cell = 0; cell < RiskRules.Posts; cell++)
            {
                double blow = health[cell] > 0 ? rules.GuardHit(guards[cell]) * aim : 0;
                while (blow > 0 && foes.Count > 0)
                {
                    int target = guards[cell] == RiskRules.Dog ? foes.Count - 1 : 0;
                    double taken = Math.Min(blow, foeHealth[target]);
                    foeHealth[target] -= taken;
                    blow = guards[cell] == RiskRules.Baton ? blow - taken : 0;
                    if (foeHealth[target] <= 1e-9) { foes.RemoveAt(target); foeHealth.RemoveAt(target); }
                }
            }
        }

        /// <summary>The order the gang fills the five posts across: the middle of the gateway first, then outwards.</summary>
        private static readonly int[] Rush = { 2, 1, 3, 0, 4 };

        /// <summary>The gang spreads over the five posts across. The front guard of a post holds one man, a tank two;
        /// whoever is not held hits the gate itself. Agitators stir the rest up, a negotiator cools them by a fifth.</summary>
        private void GangStrikes()
        {
            double morale = (1 + rules.Get("provokeMorale") * foes.Count(kind => kind == RiskRules.Agitator)) *
                (Standing(RiskRules.Talker) ? rules.Get("talkerMorale") : 1);
            var held = new int[RiskRules.Columns];
            for (int i = 0; i < foes.Count; i++)
            {
                int column = Rush[i % RiskRules.Columns], front = Front(column);
                double blow = rules.FoeHit(foes[i]) * morale;
                if (front < 0 || held[column] >= (guards[front] == RiskRules.Tank ? 2 : 1)) { gate -= blow; continue; }
                held[column]++;
                health[front] -= blow;
            }
        }

        /// <summary>The standing guard nearest the street in a column, or -1 when the post is open.</summary>
        private int Front(int column)
        {
            for (int cell = column; cell < RiskRules.Posts; cell += RiskRules.Columns)
                if (guards[cell] > 0 && health[cell] > 0) return cell;
            return -1;
        }
    }
}
