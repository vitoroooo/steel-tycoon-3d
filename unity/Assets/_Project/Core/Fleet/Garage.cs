using System;
using System.Collections.Generic;
using System.Linq;
using Steel.Core.State.Slice;

namespace Steel.Core.Fleet
{
    /// <summary>Every drivable vehicle in the city, bought or still standing at the dealer.</summary>
    public sealed class Garage
    {
        private readonly MotorState[] motors;
        public IReadOnlyList<MotorState> All => motors;

        public Garage(params MotorRules[] rules)
        {
            if (rules == null || rules.Length == 0) throw new ArgumentException("Garage needs a vehicle.");
            if (rules.Select(r => r.Id).Distinct().Count() != rules.Length)
                throw new ArgumentException("Vehicle ids must be unique.");
            motors = rules.Select(r => new MotorState(r)).ToArray();
        }

        public MotorState Driven => motors.FirstOrDefault(m => m.Occupied);

        public MotorState Find(string id) =>
            motors.FirstOrDefault(m => m.Rules.Id == id) ??
            throw new ArgumentException("Unknown vehicle: " + id);

        public MotorState Nearest(WorldPoint point) =>
            motors.OrderBy(m => m.Position.Distance(point)).First();

        public void AdvanceTrip(double seconds)
        {
            foreach (var motor in motors) motor.AdvanceTrip(seconds);
        }
    }
}
