using System;
using Steel.Core.Scrap;
using Steel.Core.State;
using Steel.Core.State.Slice;
using Steel.Core.State.Saving;

namespace Steel.Core.Fleet
{
    /// <summary>One drivable vehicle: who owns it, where it stands and what it carries.</summary>
    public sealed class MotorState
    {
        public MotorRules Rules { get; }
        public Backpack Cargo { get; }
        public WorldPoint Position { get; private set; }
        public bool Owned { get; private set; }
        public bool Occupied { get; private set; }
        public double Speed { get; private set; }
        public double RemainingTripSeconds { get; private set; }

        public MotorState(MotorRules rules)
        {
            Rules = rules; Cargo = new Backpack(rules.CapacityKg); Position = rules.Spawn;
            RemainingTripSeconds = rules.TripRealSeconds;
        }

        public bool InReach(WorldPoint player) => player.Distance(Position) <= Rules.EnterRange;

        internal MotorSave Snapshot() => new MotorSave { id = Rules.Id, owned = Owned, occupied = Occupied,
            x = Position.X, z = Position.Z, seconds = RemainingTripSeconds, cargo = Cargo.Snapshot() };

        internal void Load(MotorSave dto)
        {
            if (dto.occupied && !dto.owned) throw new FormatException("Unowned driver");
            Owned = dto.owned; Occupied = dto.occupied; Position = new WorldPoint(dto.x, dto.z);
            RemainingTripSeconds = NumberRules.NonNegative(dto.seconds, "seconds"); Cargo.Load(dto.cargo);
        }

        public void Buy()
        {
            if (Owned) throw new InvalidOperationException("Anda sudah memiliki " + Rules.Label + ".");
            Owned = true;
        }

        public void Enter(WorldPoint player)
        {
            if (!Owned) throw new InvalidOperationException("Beli " + Rules.Label + " dahulu.");
            if (Occupied || !InReach(player))
                throw new InvalidOperationException("Dekati " + Rules.Label + " terlebih dahulu.");
            Occupied = true;
            if (RemainingTripSeconds == 0) RemainingTripSeconds = Rules.TripRealSeconds;
        }

        public void Exit(WorldPoint player)
        {
            if (!Occupied || Math.Abs(Speed) > Rules.ExitMaxSpeed)
                throw new InvalidOperationException("Hentikan " + Rules.Label + " sebelum turun.");
            if (player.Distance(Position) > Rules.EnterRange + 1)
                throw new InvalidOperationException("Posisi keluar terlalu jauh.");
            Occupied = false; Speed = 0;
        }

        public void ReportPose(WorldPoint position, double speed)
        {
            if (double.IsNaN(speed) || double.IsInfinity(speed) || speed > Rules.MaxSpeed || speed < -Rules.ReverseSpeed)
                throw new ArgumentOutOfRangeException(nameof(speed));
            if (!Occupied && speed != 0)
                throw new InvalidOperationException(Rules.Label + " belum dikemudikan.");
            Position = position; Speed = speed;
        }

        public void AdvanceTrip(double seconds)
        {
            NumberRules.NonNegative(seconds, nameof(seconds));
            if (Occupied) RemainingTripSeconds = Math.Max(0, RemainingTripSeconds - seconds);
        }
    }
}
