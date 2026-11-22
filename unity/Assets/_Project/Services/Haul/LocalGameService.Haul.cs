using System;
using System.Linq;
using Steel.Core.Fleet;
using Steel.Core.Scrap;
using Steel.Core.State.Slice;
using Steel.Core.Trade.Haul;
using Steel.Services.Map;

namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        private void CreateHaul(IConfigSource source)
        {
            MapPlan plan = MapPlan.Load();
            session.OpenHaul(new HaulRules(source.Read("Haul"), Post(plan, MapSite.PlayerLapak), Post(plan, MapSite.MillGate)));
        }

        private static WorldPoint Post(MapPlan plan, string kind)
        {
            MapSite site = plan.sites.First(s => s.kind == kind);
            var at = site.Point(MapSite.PostRight, MapSite.PostForward);
            return new WorldPoint(at.x, at.y);
        }

        public SliceLocation YardPost => new SliceLocation(session.Haul.Yard);
        public SliceLocation GatePost => new SliceLocation(session.Haul.Gate);
        public HaulView YardDock => Dock(session.Haul.Yard, session.Haul.YardReach);
        public HaulView GateDock => Dock(session.Haul.Gate, session.Haul.GateReach);
        private double GatePrice(int grade) => scrap.UnitRate((ScrapGrade)grade, 0, Prices.BuyFromK2);

        private HaulView Dock(WorldPoint dock, double reach)
        {
            MotorState motor = session.TruckAt(dock, reach);
            var view = new HaulView { Stock = session.Yard.stock.ToArray(), Notice = session.LoaderStopped };
            if (motor == null) return view;
            TruckLoad load = session.LoadOn(motor.Rules.Id);
            view.Truck = motor.Rules.Id; view.TruckName = motor.Rules.Label; view.CapacityKg = motor.Rules.CapacityKg;
            view.RunCost = session.Haul.RunCost(motor.Rules.Id);
            if (load == null) return view;
            view.GradeIndex = load.grade; view.Grade = ScrapText.Grade((ScrapGrade)load.grade);
            view.LoadKg = session.OnBed(load, GameHours); view.TargetKg = load.targetKg;
            view.Loading = load.Loading; view.Unloading = load.Unloading; view.PerKg = GatePrice(load.grade);
            view.Net = load.Unloading ? load.payout : session.GateNet(load, view.PerKg);
            view.HoursLeft = load.Unloading ? Math.Max(0, load.doneHour - GameHours)
                : (load.targetKg - load.kg) / session.Maintenance.LoaderRate;
            return view;
        }

        public double BedShare(string truck)
        {
            TruckLoad load = session.LoadOn(truck);
            return load == null ? 0 : session.OnBed(load, GameHours) / session.Fleet.Find(truck).Rules.CapacityKg;
        }

        private string DockedTruck(HaulView dock, string where) =>
            dock.Truck ?? throw new InvalidOperationException("Parkir truk engkel, tronton atau trailer Anda di " + where + ".");

        public void LoadTruck(int grade)
        {
            string stop = session.CrewStopReason(GameHours, session.HasLoader ? session.Yard.Machine(true, GameHours).condition : 0);
            if (stop != null) throw new InvalidOperationException(stop);
            session.StartLoading(DockedTruck(YardDock, "kavling lapak"), grade, GameHours);
        }

        public void StopLoading() => session.StopLoading(DockedTruck(YardDock, "kavling lapak"));

        public double WeighIn()
        {
            HaulView dock = GateDock;
            return session.WeighIn(DockedTruck(dock, "jembatan timbang"), dock.PerKg, GameHours);
        }

        private InteractionView NearbyPost()
        {
            if (session.Position.Distance(session.Haul.Gate) <= session.Haul.DeskReach)
                return new InteractionView { Id = "gate", Title = "Pos timbang PT Baja Nusantara", Detail = "Timbang dan bongkar muatan truk" };
            if (session.Yard.bandar && session.Position.Distance(session.Haul.Yard) <= session.Haul.DeskReach)
                return new InteractionView { Id = "yardpost", Title = "Pos muat kavling Anda", Detail = "Muat stok lapak ke truk dengan loader" };
            return null;
        }
    }
}
