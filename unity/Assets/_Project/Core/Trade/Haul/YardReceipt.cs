namespace Steel.Core.Trade.Haul
{
    /// <summary>The latest lot a collector weighed in at the player's yard. Serial counts every lot bought this session.</summary>
    public readonly struct YardReceipt
    {
        public readonly long Serial;
        public readonly int Grade;
        public readonly double Kg, Paid;

        public YardReceipt(long serial, int grade, double kg, double paid)
        {
            Serial = serial; Grade = grade; Kg = kg; Paid = paid;
        }
    }
}
