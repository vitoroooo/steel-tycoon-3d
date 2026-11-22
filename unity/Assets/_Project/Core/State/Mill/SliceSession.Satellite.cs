using System;
using System.Linq;
using Steel.Core.State.Saving;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public SatelliteSave[] Satellites { get; private set; } = Array.Empty<SatelliteSave>();
        public SatelliteSave SiteAt(int district) => Satellites.FirstOrDefault(s => s.district == district);
        internal void LoadSites(SatelliteSave[] sites)
        {
            Satellites = sites ?? Array.Empty<SatelliteSave>();
            if (Satellites.Length > 2 || Satellites.Any(x => x == null) ||
                Satellites.Select(x => x.district).Distinct().Count() != Satellites.Length)
                throw new FormatException("Daftar satelit tidak valid.");
            foreach (var site in Satellites) site.Validate();
        }
        public void LeaseSite(int district,double hour,ConfigValues rules)
        {
            RequireNear(Rules.Factory);
            if (!Mill.opened || district < 1 || district > 2) throw new InvalidOperationException("Khusus smelter.");
            var site = SiteAt(district);
            if (site != null && site.leaseUntil > hour) throw new InvalidOperationException("Sewa masih aktif.");
            double equipment = site == null ? rules.Positive("loader")+rules.Positive("scale") : 0;
            Pay(rules.Positive("rent")+equipment);
            if (site == null)
            {
                site = new SatelliteSave {district = district}; Satellites = Satellites.Concat(new[]{site}).ToArray();
            }
            site.leaseUntil = hour+rules.Positive("leaseHours");
        }
        public void SiteBid(int district,double hour,double bid)
        {
            RequireNear(Rules.Factory); var site = SiteAt(district);
            if (site == null || hour >= site.leaseUntil) throw new InvalidOperationException("Sewa tidak aktif.");
            site.bid = NumberRules.NonNegative(bid,"bid");
        }
        public void SupplySites(long hour,double floor,ConfigValues rules)
        {
            if (!Mill.opened) return;
            foreach (var site in Satellites)
            {
                if (hour%4 != site.district || hour >= site.leaseUntil || site.bid < floor) continue;
                double kg = rules.Positive("npcLotKg"), cost = kg*site.bid;
                if (Cash < cost || site.stock+kg > rules.Positive("capacityKg")) continue;
                Pay(cost); site.stock += kg;
            }
        }
        public void TransferSite(int district,ConfigValues rules)
        {
            RequireNear(Rules.Factory); var site = SiteAt(district); double kg = rules.Positive("minShipmentKg");
            if (site == null || site.stock < kg || Yard.stock.Sum()+kg > rules.Positive("capacityKg"))
                throw new InvalidOperationException("Perlu 15 ton HMS 2 di satelit dan ruang di lapak pusat.");
            Pay(rules.Positive("shipFee")); site.stock -= kg; Yard.stock[1] += kg;
        }
    }
}
