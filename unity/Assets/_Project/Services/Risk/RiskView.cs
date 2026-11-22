namespace Steel.Services
{
    /// <summary>The yard's trouble as the security desk and the gate show it.</summary>
    public struct RiskView
    {
        /// <summary>The player runs a Kasta 2 yard, so trouble can find it.</summary>
        public bool Open;
        public double Heat, RaidChance, QuietHours, ProtectorHours, ProtectorFee;
        /// <summary>Today's shady load, if any: its name, weight, what it earns unseen and the fine when raided.</summary>
        public bool Offer;
        public string OfferName, Note;
        public double OfferKg, OfferGain, OfferFine;
        /// <summary>A gang at the gate: how many, what they ask, how long they wait, whether talks were tried.</summary>
        public bool Demand, Haggled, AutoDefend;
        public int Thugs;
        public double DemandDue, DemandHoursLeft;
        /// <summary>The 5x4 gate grid, lane by lane: 0 empty, 1 shield, 2 baton, 3 calmer; and what hiring each kind costs.</summary>
        public int[] Guards;
        public double[] GuardCost;
    }

    public partial interface IGameService
    {
        RiskView Risk { get; }
        /// <summary>0 takes today's shady load, 1 refuses it, 2 reports it. Returns what happened.</summary>
        string AnswerContraband(int choice);
        /// <summary>0 pays the gang, 1 talks, 2 holds the gate. Returns what happened.</summary>
        string AnswerGang(int choice);
        void PostGuard(int cell, int kind);
        void ToggleAutoDefend();
        void HireProtector();
    }
}
