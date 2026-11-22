namespace Steel.Services
{
    /// <summary>What a loading or weighing post shows about the truck standing at it.</summary>
    public sealed class HaulView
    {
        /// <summary>Null when none of the player's trucks is parked at the post.</summary>
        public string Truck, TruckName, Grade, Notice;
        public int GradeIndex;
        public bool Loading, Unloading;
        public double CapacityKg, LoadKg, TargetKg, HoursLeft, Net, RunCost, PerKg;
        /// <summary>Yard stock per grade, for choosing what to load.</summary>
        public double[] Stock;
    }

    public partial interface IGameService
    {
        /// <summary>The loading post on the player's yard lot and the weighing post at the mill gate.</summary>
        SliceLocation YardPost { get; }
        SliceLocation GatePost { get; }
        HaulView YardDock { get; }
        HaulView GateDock { get; }
        /// <summary>How full this truck's bed is with yard stock right now, 0 to 1.</summary>
        double BedShare(string truck);
        void LoadTruck(int grade);
        void StopLoading();
        double WeighIn();
    }
}
