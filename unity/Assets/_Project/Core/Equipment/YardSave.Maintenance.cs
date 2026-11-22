using System;
using Steel.Core.Equipment;

namespace Steel.Core.State.Saving
{
    public sealed partial class YardSave
    {
        // The fixed order is scale, loader. Old career saves have no array yet.
        public MachineState[] machines;
        [NonSerialized] private bool machinesChecked;

        private void CheckMachines()
        {
            if (machinesChecked) return;
            if (machines == null || machines.Length == 0)
                machines = new[] { new MachineState { randomState = 31 }, new MachineState { randomState = 73 } };
            if (machines.Length != 2 || machines[0] == null || machines[1] == null)
                throw new FormatException("Daftar alat lapak tidak valid.");
            foreach (var machine in machines) machine.Validate();
            machinesChecked = true;
        }

        public MachineState Machine(bool loader, double hour)
        {
            CheckMachines();
            var machine = machines[loader ? 1 : 0];
            machine.Advance(hour);
            return machine;
        }
    }
}
