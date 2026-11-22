// Written by server/career (generate). Do not edit: change IGameService or RemoteCalls and run it again.
namespace Steel.Services.Remote
{
    public sealed partial class RemoteGameService
    {
        public int BuyerCount => game.BuyerCount;
        public global::Steel.Services.CartView Cart => game.Cart;
        public global::Steel.Services.EventView Events => game.Events;
        public double GameHours => game.GameHours;
        public global::Steel.Services.HaulView GateDock => game.GateDock;
        public global::Steel.Services.SliceLocation GatePost => game.GatePost;
        public int GateQueue => game.GateQueue;
        public bool GuideRead => game.GuideRead;
        public bool HasSteel => game.HasSteel;
        public global::Steel.Services.IncomeView Income => game.Income;
        public global::Steel.Services.YardVisit LastVisit => game.LastVisit;
        public string[] Legacies => game.Legacies;
        public bool LegacyOpen => game.LegacyOpen;
        public bool MotorRoute => game.MotorRoute;
        public global::Steel.Services.InteractionView Nearby => game.Nearby;
        public global::Steel.Services.SellerView NearSeller => game.NearSeller;
        public double NextBikeMinutes => game.NextBikeMinutes;
        public global::Steel.Core.Economy.AiQuote Prices => game.Prices;
        public bool RainHere => game.RainHere;
        public bool ReadWeather => game.ReadWeather;
        public global::Steel.Services.ReuseView Reuse => game.Reuse;
        public global::Steel.Services.RiskView Risk => game.Risk;
        public int SellerCount => game.SellerCount;
        public global::Steel.Services.SpkView Spk => game.Spk;
        public global::Steel.Services.SpkOfferView[] SpkBoard => game.SpkBoard;
        public bool StoredUnderRoof => game.StoredUnderRoof;
        public int Story => game.Story;
        public bool StreetTipRead => game.StreetTipRead;
    }
}
