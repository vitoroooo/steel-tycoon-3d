using System;
using Steel.Core.Fleet;
using Steel.Core.Scrap;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        private readonly Backpack backpack;
        public PickupState Pickup { get; }
        public Backpack Bag => Pickup.Occupied ? Pickup.Cargo : Cart.Held ? Cart.Cargo : backpack;
        public void EnterPickup()
        {
            if (Cart.Held) throw new InvalidOperationException("Lepas gerobak dahulu.");
            if (Pickup.Cargo.CarriedKg + backpack.CarriedKg > Pickup.Rules.CapacityKg)
                throw new InvalidOperationException("Bak pikap penuh.");
            Pickup.Enter(Position);
            foreach (var pile in backpack.Items) Pickup.Cargo.Add(pile);
            backpack.Clear(); Position = Pickup.Position;
        }
        public void ExitPickup(WorldPoint point)
        {
            Pickup.Exit(point); Position = point;
        }
        public void ReportPickup(WorldPoint point, double speed)
        {
            Pickup.ReportPose(point, speed);
            if (Pickup.Occupied) Position = point;
        }
    }
}
