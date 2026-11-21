using System;
using Steel.Core.Scrap;
using Steel.Core.State;
using Steel.Core.State.Slice;

namespace Steel.Core.Fleet
{
    public sealed class PickupState
    {
        public PickupRules Rules { get; }
        public Backpack Cargo { get; }
        public WorldPoint Position { get; private set; }
        public bool Occupied { get; private set; }
        public double Speed { get; private set; }
        public double RemainingTripSeconds { get; private set; }
        public PickupState(PickupRules rules)
        {
            Rules = rules; Cargo = new Backpack(rules.CapacityKg); Position = rules.Spawn;
            RemainingTripSeconds = rules.TripRealSeconds;
        }
        public void Enter(WorldPoint player)
        {
            if (Occupied || player.Distance(Position) > Rules.EnterRange)
                throw new InvalidOperationException("Dekati pintu pikap terlebih dahulu.");
            Occupied = true;
            if (RemainingTripSeconds == 0) RemainingTripSeconds = Rules.TripRealSeconds;
        }
        public void Exit(WorldPoint player)
        {
            if (!Occupied || Math.Abs(Speed) > Rules.ExitMaxSpeed)
                throw new InvalidOperationException("Hentikan pikap sebelum keluar.");
            if (player.Distance(Position) > Rules.EnterRange + 1)
                throw new InvalidOperationException("Posisi keluar terlalu jauh.");
            Occupied = false; Speed = 0;
        }
        public void ReportPose(WorldPoint position, double speed)
        {
            if (double.IsNaN(speed) || double.IsInfinity(speed) || speed > Rules.MaxSpeed || speed < -Rules.ReverseSpeed)
                throw new ArgumentOutOfRangeException(nameof(speed));
            if (!Occupied && speed != 0) throw new InvalidOperationException("Pikap belum dikemudikan.");
            Position = position; Speed = speed;
        }
        public void AdvanceTrip(double seconds)
        {
            NumberRules.NonNegative(seconds, nameof(seconds));
            if (Occupied) RemainingTripSeconds = Math.Max(0, RemainingTripSeconds - seconds);
        }
    }
}
