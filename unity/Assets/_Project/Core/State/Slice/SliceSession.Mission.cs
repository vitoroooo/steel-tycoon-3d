using System;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public bool FirstSaleMade { get; private set; }
        public bool GrantCollected { get; private set; }
        public bool GrantReady => FirstSaleMade && !GrantCollected;

        public double ClaimDealerGrant(string starterVehicle)
        {
            if (!GrantReady) throw new InvalidOperationException("Hadiah belum tersedia atau sudah diambil.");
            if (!Fleet.Find(starterVehicle).InReach(Position))
                throw new InvalidOperationException("Ambil hadiah Pak Darto di samping motor bak di dealer.");
            GrantCollected = true;
            CreditCash(Rules.FirstSaleGrant);
            return Rules.FirstSaleGrant;
        }
    }
}
