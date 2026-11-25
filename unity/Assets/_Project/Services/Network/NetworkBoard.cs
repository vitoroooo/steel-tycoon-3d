using System;

namespace Steel.Services
{
    [Serializable]
    public sealed class NetworkBoard
    {
        public long id, cash, stock, gameHour;
        public int ibn, minimum, maximum, reference;
        public string name;
        public NetworkLot[] lots;
    }

    [Serializable]
    public sealed class NetworkLot
    {
        public long id, seller;
        public int kg, price;
        public string name;
        public long Gross => (long)kg * price;
    }

    [Serializable]
    internal sealed class NetworkReceipt
    {
        public long lot = 0;
        public string message = "";
    }

    [Serializable]
    internal sealed class SellCommand
    {
        public string key = Guid.NewGuid().ToString("N");
        public string action = "sell";
        public int kg, price;
    }

    [Serializable]
    internal sealed class LotCommand
    {
        public string key = Guid.NewGuid().ToString("N");
        public string action;
        public long lot;
    }
}
