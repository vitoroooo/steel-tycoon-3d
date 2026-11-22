using Steel.Core.State.Saving;

namespace Steel.Services
{
    public struct TenderView
    {
        public long Round;
        public string Notice;
        public bool Open, Awarded;
        public double UnitPrice, Delivered, Left, Hours;
    }
    public partial interface IGameService
    {
        TenderView GovernmentOrder(int priceStep);
        void OfferBridgeSteel(int priceStep);
        double DeliverBridgeSteel();
    }
    public sealed partial class LocalGameService
    {
        public TenderView GovernmentOrder(int priceStep)
        {
            var t = session.Tender; long round = TenderSave.Current(GameHours);
            double age = GameHours-round*TenderSave.Interval;
            return new TenderView { Round = round+1, Notice = t.last, Awarded = t.awarded, Delivered = t.sent,
                Open = age < TenderSave.Closing && !t.awarded && !(t.offered && t.round == round),
                Left = t.Remaining, Hours = t.awarded ? t.round*TenderSave.Interval+TenderSave.Deadline-GameHours :
                    age < TenderSave.Closing ? TenderSave.Closing-age : TenderSave.Interval-age,
                UnitPrice = t.awarded ? t.bid : BeamBid(priceStep) };
        }
        private double BeamBid(int step)
        {
            if (step < 0 || step > 2) throw new System.ArgumentOutOfRangeException(nameof(step));
            return System.Math.Floor(Prices.Ibn * millRules.Positive("beamRate")*(.85+.10*step));
        }
        public void OfferBridgeSteel(int priceStep) => session.BidTender(GameHours,BeamBid(priceStep),false);
        public double DeliverBridgeSteel() => session.DeliverTender();
        private void TenderHour(long hour) => session.ResolveTender(hour,Prices.Ibn*millRules.Positive("beamRate"));
    }
}
