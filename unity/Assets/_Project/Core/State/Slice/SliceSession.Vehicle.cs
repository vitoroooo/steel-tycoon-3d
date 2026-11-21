using System;
using Steel.Core.Fleet;
using Steel.Core.Scrap;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        private readonly Backpack backpack;
        public Garage Fleet { get; }

        /// <summary>Where picked-up scrap goes: the driven vehicle, the pushed cart, or the hands.</summary>
        public Backpack Bag =>
            Fleet.Driven != null ? Fleet.Driven.Cargo : Cart.Held ? Cart.Cargo : backpack;

        public void BuyMotor(string id)
        {
            var motor = Fleet.Find(id);
            if (!motor.InReach(Position))
                throw new InvalidOperationException("Dekati " + motor.Rules.Label + " terlebih dahulu.");
            if (!motor.Owned && Cash < motor.Rules.Price)
                throw new InvalidOperationException("Kas belum cukup untuk membeli " + motor.Rules.Label + ".");
            motor.Buy(); Cash -= motor.Rules.Price;
        }

        public void EnterMotor(string id)
        {
            var motor = Fleet.Find(id);
            if (Cart.Held) throw new InvalidOperationException("Lepas gerobak dahulu.");
            if (Routes.Busy(id)) throw new InvalidOperationException("Armada sedang menjalankan rute.");
            if (Fleet.Driven != null) throw new InvalidOperationException("Turun dari kendaraan dahulu.");
            if (motor.Cargo.CarriedKg + backpack.CarriedKg > motor.Rules.CapacityKg)
                throw new InvalidOperationException("Bak " + motor.Rules.Label + " penuh.");
            motor.Enter(Position);
            foreach (var pile in backpack.Items) motor.Cargo.Add(pile);
            backpack.Clear(); Position = motor.Position;
        }

        public void ExitMotor(WorldPoint point)
        {
            var motor = Fleet.Driven ?? throw new InvalidOperationException("Anda tidak sedang berkendara.");
            motor.Exit(point); Position = point;
        }

        public void ReportMotor(string id, WorldPoint point, double speed, double? heading = null)
        {
            var motor = Fleet.Find(id); motor.ReportPose(point, speed, heading);
            if (motor.Occupied) Position = point;
        }
    }
}
