namespace Steel.Services
{
    /// <summary>"Temuan barang reuse" at the yard as the Buku usaha shows it.</summary>
    public struct ReuseView
    {
        /// <summary>A find waiting: kg, what Toko Konstruksi pays for it now, game hours until it sells itself.</summary>
        public bool Found;
        public double Kg, Price, HoursLeft;
        /// <summary>Goods kept for a special SPK and what Toko Konstruksi would pay for them today.</summary>
        public double StoreKg, StorePrice;
        /// <summary>A special SPK for the kept goods: kg wanted, payment, game hours left.</summary>
        public bool Order;
        public double OrderKg, OrderPay, OrderHoursLeft;
        public bool Any => Found || StoreKg > 0;
    }

    public partial interface IGameService
    {
        ReuseView Reuse { get; }
        /// <summary>Sells the find to Toko Konstruksi (true) or keeps it for a special SPK. Returns what happened.</summary>
        string AnswerReuse(bool sell);
        /// <summary>Sends the kept goods to the special SPK. Returns the payment.</summary>
        double DeliverReuseOrder();
        /// <summary>Sells the kept goods to Toko Konstruksi at the lowest reuse price. Returns the payment.</summary>
        double SellReuseStore();
    }
}
