using System;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public void InstallRoof(bool concrete, double cost)
        {
            RequireNear(Rules.Home);
            if (!Yard.bandar) throw new InvalidOperationException("Perlu lapak Kasta 2.");
            if (concrete ? Yard.paved : Yard.hangar) throw new InvalidOperationException("Sudah terpasang.");
            Pay(cost); if (concrete) Yard.paved = true; else Yard.hangar = true;
        }
        private void AdmitTrailer(double hour, int soil)
        {
            if (Yard.stuck) throw new InvalidOperationException("Trailer terjebak. Pilih loader atau derek.");
            if (Yard.rescued && hour < Yard.rescueHour) throw new InvalidOperationException("Loader masih bekerja.");
            bool cleared = Yard.rescued; Yard.rescued = false;
            uint seed = unchecked((uint)(++Yard.entries * 747796405));
            seed = (seed ^ seed >> 16) * 2246822519u;
            if (!cleared && soil == 2 && !Yard.paved && seed % 100 < 40)
            {
                Yard.stuck = true;
                throw new InvalidOperationException("Trailer terjebak lumpur. Buka halaman Cuaca.");
            }
        }
        public void Rescue(bool tow, double hour, double price)
        {
            RequireNear(Rules.Home);
            if (!Yard.stuck) throw new InvalidOperationException("Tidak ada trailer terjebak.");
            if (tow) Pay(price);
            else WorkLoader(hour, 1, 1);
            Yard.stuck = false; Yard.rescued = true; Yard.rescueHour = hour + (tow ? 0 : 1);
        }
    }
}
