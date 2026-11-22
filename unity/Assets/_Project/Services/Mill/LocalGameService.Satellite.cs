namespace Steel.Services
{
    public struct SatelliteView
    {
        public string Area;
        public bool Leased;
        public double Stock, Bid, Floor, Fee;
    }
    public partial interface IGameService
    {
        SatelliteView Satellite(int district);
        void LeaseSatellite(int district);
        void PriceSatellite(int district,bool active);
        void HaulSatellite(int district);
    }
    public sealed partial class LocalGameService
    {
        public SatelliteView Satellite(int district)
        {
            var s = session.SiteAt(district);
            return new SatelliteView { Area = Areas[district], Leased = s != null && s.leaseUntil > GameHours,
                Stock = s?.stock ?? 0, Bid = s?.bid ?? 0, Floor = CounterRate(1),
                Fee = business.Positive("rent")+(s != null ? 0 : business.Positive("scale")+business.Positive("loader")) };
        }
        public void LeaseSatellite(int district) => session.LeaseSite(district,GameHours,business);
        public void PriceSatellite(int district,bool active) => session.SiteBid(district,GameHours,
            active ? System.Math.Ceiling(CounterRate(1)*1.05) : 0);
        public void HaulSatellite(int district) => session.TransferSite(district,business);
        private void SiteHour(long hour) => session.SupplySites(hour,CounterRate(1),business);
    }
}
