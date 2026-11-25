// Written by server/career (generate). Do not edit: change IGameService or RemoteCalls and run it again.
namespace Steel.Services.Remote
{
    public sealed partial class RemoteGameService
    {
        public bool StreetTipRead => game.StreetTipRead;
        public global::Steel.Services.VehicleView Vehicle => game.Vehicle;
        public global::Steel.Services.GameView View => game.View;
        public global::Steel.Services.HaulView YardDock => game.YardDock;
        public global::Steel.Services.SliceLocation YardPost => game.YardPost;
        public string AnswerContraband(int choice) { var value = game.AnswerContraband(choice); Sent("AnswerContraband", choice); return value; }
        public string AnswerDeposit(bool accept) { var value = game.AnswerDeposit(accept); Sent("AnswerDeposit", accept); return value; }
        public string AnswerGang(int choice) { var value = game.AnswerGang(choice); Sent("AnswerGang", choice); return value; }
        public string AnswerReuse(bool sell) { var value = game.AnswerReuse(sell); Sent("AnswerReuse", sell); return value; }
        public void ApplyAmdal(bool complete) { game.ApplyAmdal(complete); Sent("ApplyAmdal", complete); }
        public void ApplyPermit(bool matched) { game.ApplyPermit(matched); Sent("ApplyPermit", matched); }
        public void ApplySafetyCertificate() { game.ApplySafetyCertificate(); Sent("ApplySafetyCertificate"); }
        public double BedShare(string truck) => game.BedShare(truck);
        public void BookRoute(global::Steel.Services.RouteView preview) { game.BookRoute(preview); Sent("BookRoute", preview); }
        public void BuildMill() { game.BuildMill(); Sent("BuildMill"); }
        public double BuyAtDoor(string id) { var value = game.BuyAtDoor(id); Sent("BuyAtDoor", id); return value; }
        public global::Steel.Services.MarketView Buyer(int index) => game.Buyer(index);
        public void BuyScanner(bool xrf) { game.BuyScanner(xrf); Sent("BuyScanner", xrf); }
        public void BuyVehicle(string id) { game.BuyVehicle(id); Sent("BuyVehicle", id); }
        public void ChargeFurnace(int product, int grade) { game.ChargeFurnace(product, grade); Sent("ChargeFurnace", product, grade); }
        public double ClaimDealerGrant() { var value = game.ClaimDealerGrant(); Sent("ClaimDealerGrant"); return value; }
        public string Classify(string id, int bin, bool scan) { var value = game.Classify(id, bin, scan); Sent("Classify", id, bin, scan); return value; }
        public double CollectRoute() { var value = game.CollectRoute(); Sent("CollectRoute"); return value; }
        public double ConfirmSale() { var value = game.ConfirmSale(); Sent("ConfirmSale"); return value; }
        public global::Steel.Core.Scrap.ScrapPile CreatePile(double kilograms, global::Steel.Core.Scrap.ScrapGrade grade) => game.CreatePile(kilograms, grade);
        public global::Steel.Services.CrewView CrewInfo() => game.CrewInfo();
        public void CycleCrewPolicy() { game.CycleCrewPolicy(); Sent("CycleCrewPolicy"); }
        public double DeliverBridgeSteel() { var value = game.DeliverBridgeSteel(); Sent("DeliverBridgeSteel"); return value; }
    }
}
