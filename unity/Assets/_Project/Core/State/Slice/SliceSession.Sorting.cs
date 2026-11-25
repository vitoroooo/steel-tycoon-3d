using System;
using Steel.Core.Scrap;
using Steel.Core.State.Saving;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public bool Magnet { get; private set; }
        public bool Xrf { get; private set; }
        public YardSave Yard { get; private set; } = new YardSave();
        public void BuyScanner(bool xrf, double price)
        {
            RequireNear(Rules.Home);
            if (xrf && !Yard.bandar) throw new InvalidOperationException("XRF tersedia di Kasta 2.");
            if (xrf ? Xrf : Magnet) throw new InvalidOperationException("Alat sudah dimiliki.");
            if (Cash < price) throw new InvalidOperationException("Kas belum cukup.");
            Cash -= price; if (xrf) Xrf = true; else Magnet = true;
        }
        private ScrapPile Sample(string id)
        {
            if (id != null) return Available(id).Scrap;
            RequireNear(Rules.Home);
            return Bag.NextUnsorted ?? throw new InvalidOperationException("Tidak ada muatan mentah.");
        }
        public int MaterialIndex(string id) => (int)Sample(id).Material;
        public bool Magnetic(string id)
        {
            if (!Magnet) throw new InvalidOperationException("Beli magnet di meja usaha lapak.");
            return Sample(id).Material != ScrapGrade.Dirt;
        }
        public double Classify(string id, int bin, bool scan)
        {
            var pile = Sample(id);
            if (scan && !Xrf) throw new InvalidOperationException("XRF belum dimiliki.");
            if (!scan && bin != (int)pile.Material)
                throw new InvalidOperationException("Belum cocok. Perhatikan bentuk dan ketebalannya.");
            if (id != null) pile.Inspect(); else Bag.SortNext();
            return pile.WasteKg;
        }
    }
}
