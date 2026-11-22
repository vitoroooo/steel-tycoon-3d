// Written by server/career (generate). Do not edit: change IGameService or RemoteCalls and run it again.
namespace Steel.Services.Remote
{
    public sealed partial class RemoteGameService
    {
        public double DeliverReuseOrder() { var value = game.DeliverReuseOrder(); Sent("DeliverReuseOrder"); return value; }
        public void EnterVehicle(string id) { game.EnterVehicle(id); Sent("EnterVehicle", id); }
        public void ExitVehicle(float x, float z) { game.ExitVehicle(x, z); Sent("ExitVehicle", x, z); }
        public global::Steel.Services.FinanceView Finances() => game.Finances();
        public void FundCrewCsr() { game.FundCrewCsr(); Sent("FundCrewCsr"); }
        public global::Steel.Services.TenderView GovernmentOrder(int priceStep) => game.GovernmentOrder(priceStep);
        public void GrabCart() { game.GrabCart(); Sent("GrabCart"); }
        public global::Steel.Services.HaggleView Haggle(string id, double perKg) { var value = game.Haggle(id, perKg); Sent("Haggle", id, perKg); return value; }
        public void HaulSatellite(int district) { game.HaulSatellite(district); Sent("HaulSatellite", district); }
        public void HireProtector() { game.HireProtector(); Sent("HireProtector"); }
        public void ImproveLand(bool pave) { game.ImproveLand(pave); Sent("ImproveLand", pave); }
        public void Inspect(string id) { game.Inspect(id); Sent("Inspect", id); }
        public global::Steel.Services.LocalInvoice InspectBid(int index) { var value = game.InspectBid(index); Sent("InspectBid", index); return value; }
        public string InspectMagnet(string id) { var value = game.InspectMagnet(id); Sent("InspectMagnet", id); return value; }
        public void InspectSeller(string id) { game.InspectSeller(id); Sent("InspectSeller", id); }
        public void InstallLine(int product) { game.InstallLine(product); Sent("InstallLine", product); }
        public bool IsCollected(string id) => game.IsCollected(id);
        public void Lease() { game.Lease(); Sent("Lease"); }
        public void LeaseSatellite(int district) { game.LeaseSatellite(district); Sent("LeaseSatellite", district); }
        public void LoadTruck(int grade) { game.LoadTruck(grade); Sent("LoadTruck", grade); }
        public global::Steel.Services.MaintenanceView MaintenanceInfo(bool loader) => game.MaintenanceInfo(loader);
        public global::Steel.Services.MillView MillInfo(int product, int grade) => game.MillInfo(product, grade);
        public bool NeedsRespawn(double y) => game.NeedsRespawn(y);
        public void OfferBridgeSteel(int priceStep) { game.OfferBridgeSteel(priceStep); Sent("OfferBridgeSteel", priceStep); }
        public void OpenYard() { game.OpenYard(); Sent("OpenYard"); }
        public bool OwnScanner(bool xrf) => game.OwnScanner(xrf);
        public void PickUp(string id) { game.PickUp(id); Sent("PickUp", id); }
        public void PostGuard(int cell, int kind) { game.PostGuard(cell, kind); Sent("PostGuard", cell, kind); }
    }
}
