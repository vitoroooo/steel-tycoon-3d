// Written by server/career (generate). Do not edit: change IGameService or RemoteCalls and run it again.
namespace Steel.Services.Remote
{
    public sealed partial class RemoteGameService
    {
        public void PostGuard(int cell, int kind) { game.PostGuard(cell, kind); Sent("PostGuard", cell, kind); }
        public global::Steel.Services.SaleView PreviewSale() { var value = game.PreviewSale(); Sent("PreviewSale"); return value; }
        public void PriceSatellite(int district, bool active) { game.PriceSatellite(district, active); Sent("PriceSatellite", district, active); }
        public void PromoteCrewForeman() { game.PromoteCrewForeman(); Sent("PromoteCrewForeman"); }
        public double Quote(global::Steel.Core.Scrap.ScrapPile pile) => game.Quote(pile);
        public void ReadForecast() { game.ReadForecast(); Sent("ReadForecast"); }
        public void ReadGuide() { game.ReadGuide(); Sent("ReadGuide"); }
        public void ReadStory() { game.ReadStory(); Sent("ReadStory"); }
        public void ReadStreetTip() { game.ReadStreetTip(); Sent("ReadStreetTip"); }
        public void ReleaseCart() { game.ReleaseCart(); Sent("ReleaseCart"); }
        public void RepairMachine(bool loader, int choice) { game.RepairMachine(loader, choice); Sent("RepairMachine", loader, choice); }
        public void ReplaceYardTool(bool loader) { game.ReplaceYardTool(loader); Sent("ReplaceYardTool", loader); }
        public void ReportVehicle(string id, float x, float z, float speed, float? heading) { game.ReportVehicle(id, x, z, speed, heading); Moved("ReportVehicle", id, x, z, speed, heading); }
        public void Rescue(bool tow) { game.Rescue(tow); Sent("Rescue", tow); }
        public global::Steel.Services.RouteView Route(string carrier, int district, bool purchased) => game.Route(carrier, district, purchased);
        public string SampleClue(string id) { var value = game.SampleClue(id); Sent("SampleClue", id); return value; }
        public global::Steel.Services.SatelliteView Satellite(int district) => game.Satellite(district);
        public double ScannerCost(bool xrf) => game.ScannerCost(xrf);
        public global::Steel.Services.SellerView Seller(string id) => game.Seller(id);
        public bool SellerStocked(string id) => game.SellerStocked(id);
        public double SellLocal() { var value = game.SellLocal(); Sent("SellLocal"); return value; }
        public double SellReuseStore() { var value = game.SellReuseStore(); Sent("SellReuseStore"); return value; }
        public double SellSteel(int product) { var value = game.SellSteel(product); Sent("SellSteel", product); return value; }
        public double SellToPlayerYard(int plot) { var value = game.SellToPlayerYard(plot); Sent("SellToPlayerYard", plot); return value; }
        public void SetBid(int grade, double price) { game.SetBid(grade, price); Sent("SetBid", grade, price); }
        public void SetPosition(float x, float z) { game.SetPosition(x, z); Moved("SetPosition", x, z); }
        public void Shelter(bool store) { game.Shelter(store); Sent("Shelter", store); }
        public double ShipGrade(int grade) { var value = game.ShipGrade(grade); Sent("ShipGrade", grade); return value; }
    }
}
