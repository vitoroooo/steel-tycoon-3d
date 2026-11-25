using System;
using Steel.Core.State;

namespace Steel.Core.Risk
{
    /// <summary>Numbers for contraband, raids, extortion and gate guards (PRD 6.3, 6.7 and appendix D), read from Risk.json.</summary>
    public sealed class RiskRules
    {
        /// <summary>The gate grid: five posts across, four rows deep. A cell is row * Columns + column; row 0 faces the street
        /// and guards stand only on the two front rows.</summary>
        public const int Columns = 5, Rows = 4, Cells = Columns * Rows, Posts = Columns * 2;
        /// <summary>Guard kinds of appendix D.2; 0 is an empty post. The escort is hired for one event only.</summary>
        public const int Tank = 1, Baton = 2, Dog = 3, Cctv = 4, Talker = 5, Escort = 6, Kinds = 6;
        /// <summary>Gang members of appendix D.3.</summary>
        public const int Youth = 1, Champion = 2, Agitator = 3, Leader = 4;
        private readonly ConfigValues values;

        public RiskRules(string json) { values = new ConfigValues(json); }

        public double Get(string key) => values.Positive(key);
        public double GuardHp(int kind) => values.Positive("guard" + kind + "Hp");
        public double GuardHit(int kind) => values.Get("guard" + kind + "Hit");
        public double GuardCost(int kind) => values.Positive("guard" + kind + "Cost");
        public double FoeHp(int kind) => values.Positive("foe" + kind + "Hp");
        public double FoeHit(int kind) => values.Positive("foe" + kind + "Hit");
        public double GateDamage(int caste) => Get(caste >= 3 ? "gateDamage3" : "gateDamage2");

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
