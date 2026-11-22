using System.Collections.Generic;

namespace Steel.Core.Risk
{
    /// <summary>Who comes in each of the gang's three waves (appendix D.3): youths throughout, champions and an agitator
    /// from the second wave, and a leader in the third against a Kasta 3 target.</summary>
    public static class GangWaves
    {
        public const int Count = 3;

        /// <summary>Each wave in walking order: the youths in front, the agitator at the back.</summary>
        public static List<int>[] For(int youths, int caste)
        {
            var waves = new List<int>[Count];
            for (int wave = 0; wave < Count; wave++)
            {
                waves[wave] = new List<int>();
                int share = youths / Count + (wave < youths % Count ? 1 : 0);
                for (int i = 0; i < share; i++) waves[wave].Add(RiskRules.Youth);
            }
            for (int i = 0; i < 1 + youths / 6; i++) waves[1].Add(RiskRules.Champion);
            if (caste >= 3) waves[2].Add(RiskRules.Leader);
            waves[1].Add(RiskRules.Agitator); waves[2].Add(RiskRules.Agitator);
            return waves;
        }
    }
}
