using System;
using Steel.Core.State;

namespace Steel.Core.Risk
{
    /// <summary>Numbers for contraband, raids, extortion and gate guards (PRD 6.3 and 6.7), read from Risk.json.</summary>
    public sealed class RiskRules
    {
        /// <summary>The gate grid: four lanes, five posts deep. A cell is lane * Columns + column; the last column faces the street.</summary>
        public const int Lanes = 4, Columns = 5, Cells = Lanes * Columns;
        /// <summary>Guard kinds: 1 shield (tank), 2 baton (close range), 3 calmer. 0 is an empty post.</summary>
        public const int Kinds = 3, Calmer = 3;
        private readonly ConfigValues values;

        public RiskRules(string json) { values = new ConfigValues(json); }

        public double Get(string key) => values.Positive(key);
        public double GuardHp(int kind) => values.Positive("guard" + kind + "Hp");
        public double GuardHit(int kind) => values.Positive("guard" + kind + "Hit");
        public double GuardCost(int kind) => values.Positive("guard" + kind + "Cost");

        /// <summary>Raid chance for one illegal load: 10% plus half a point per point of heat, halved under a protector.</summary>
        public double RaidChance(double heat, bool protector) =>
            Math.Min(1, (Get("raidBase") + heat * Get("raidPerHeat")) * (protector ? Get("protectorCut") : 1));

        /// <summary>Chance per game day that a gang turns up: more for the rich, less for a guarded gate.</summary>
        public double ExtortChance(double cash, int guards) =>
            Math.Min(Get("extortCap"), Get("extortChance") + Get("extortPerBillion") * cash / 1e9) *
            (1 - Math.Min(Get("deterMost"), Get("deterPerGuard") * guards));

        public int Thugs(double cash) =>
            (int)Math.Min(Get("thugsMost"), Get("thugsBase") + Math.Floor(Get("thugsPerBillion") * cash / 1e9));
    }
}
