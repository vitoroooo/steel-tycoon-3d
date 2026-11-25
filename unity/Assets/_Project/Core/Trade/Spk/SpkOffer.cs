namespace Steel.Core.Trade.Spk
{
    /// <summary>One work order (SPK) the mill posts for a game day: a grade, a tonnage, a fixed price and a deadline (PRD C.1).</summary>
    public readonly struct SpkOffer
    {
        /// <summary>The game day times ten plus the slot on that day's board.</summary>
        public readonly long Id;
        public readonly int Grade, MinReputation;
        public readonly double Kg, PerKg, Hours;
        public double Value => Kg * PerKg;

        public SpkOffer(long id, int grade, double kg, double perKg, double hours, int minReputation)
        {
            Id = id; Grade = grade; Kg = kg; PerKg = perKg; Hours = hours; MinReputation = minReputation;
        }
    }
}
