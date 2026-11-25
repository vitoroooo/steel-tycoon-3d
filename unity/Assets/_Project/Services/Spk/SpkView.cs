namespace Steel.Services
{
    /// <summary>One line of the mill's work-order board.</summary>
    public struct SpkOfferView
    {
        public string Grade;
        public double Tonnes, PerKg, Bond;
        public int Days, MinReputation;
        public bool Taken;
    }

    /// <summary>The work order the player is filling, and how the last one ended.</summary>
    public struct SpkView
    {
        public bool Active;
        public string Grade, Note;
        public double Tonnes, DeliveredTonnes, PerKg, Bond, HoursLeft;
        public int Reputation;
    }

    public partial interface IGameService
    {
        SpkView Spk { get; }
        /// <summary>Today's three work orders at the mill.</summary>
        SpkOfferView[] SpkBoard { get; }
        void TakeSpk(int slot);
    }
}
