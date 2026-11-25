namespace Steel.Services
{
    /// <summary>A door that sells scrap, as the dealing screen shows it.</summary>
    public sealed class SellerView
    {
        public string Id, Title, Place, Grade;
        public int Kind;
        public float X, Z;
        public bool Inspected, Weighed, Agreed, Stocked;
        /// <summary>The seller's guess until the goods were weighed, the true weight after.</summary>
        public double StockKg;
        public double AskPerKg, Step, Cash, RoomKg;
        /// <summary>What paying now would load and cost; zero when nothing fits.</summary>
        public double TakenKg, ChargedKg, Total;
        public double RestockMinutes;
    }

    /// <summary>What the seller answered to an offer.</summary>
    public sealed class HaggleView
    {
        public bool Deal;
        public string Line;
    }

    public partial interface IGameService
    {
        int SellerCount { get; }
        /// <summary>The door within reach that still has goods, or null.</summary>
        SellerView NearSeller { get; }
        SellerView Seller(string id);
        bool SellerStocked(string id);
        void InspectSeller(string id);
        void WeighSeller(string id);
        HaggleView Haggle(string id, double perKg);
        double BuyAtDoor(string id);
    }
}
