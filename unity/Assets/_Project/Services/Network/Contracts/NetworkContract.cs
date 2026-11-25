using System;

namespace Steel.Services
{
    /// <summary>A direct contract between two traders (PRD C.2), as the market server reports it.</summary>
    [Serializable]
    public sealed class NetworkContract
    {
        public const int MinDays = 3, MaxDays = 10, PenaltyPercent = 10;
        public long id, locked, due, penalty;
        public int kg, price, days, delivered, selling;
        public string seller, buyer, status;

        public bool Selling => selling != 0;
        public bool Offered => status == "offered";
        public bool Active => status == "active";
        public string Partner => Selling ? buyer : seller;
        public long Daily => (long)kg * price;
        /// <summary>What ending the contract now costs: 10% of the deliveries still to come.</summary>
        public long Penalty => Active ? (days - delivered) * Daily * PenaltyPercent / 100 : 0;
    }

    [Serializable]
    internal sealed class OfferCommand
    {
        public string key = Guid.NewGuid().ToString("N");
        public string action = "offer", buyer;
        public int kg, price, days;
    }

    [Serializable]
    internal sealed class ContractCommand
    {
        public string key = Guid.NewGuid().ToString("N");
        public string action;
        public long contract;
    }
}
