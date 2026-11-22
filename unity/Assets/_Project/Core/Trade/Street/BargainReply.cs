namespace Steel.Core.Trade.Street
{
    public enum BargainReply
    {
        /// <summary>The offer is taken; that is the price now.</summary>
        Deal,
        /// <summary>Too low, but the seller comes down a little.</summary>
        Counter,
        /// <summary>So low the seller is put off and does not move.</summary>
        Offended,
        /// <summary>No patience left: the last price asked is final.</summary>
        Final
    }
}
