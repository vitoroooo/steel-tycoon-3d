using System;
using Steel.Core.Scrap;

namespace Steel.Services
{
    public struct YardView
    {
        public int Rep, Caste;
        public bool PermitFiled, Leased;
        public double PermitHours, LeaseHours, Stock, Bid, Floor, NetShipment, Cash;
        public string Grade;
    }
    public partial interface IGameService
    {
        YardView YardInfo(int grade);
        void ApplyPermit(bool matched);
        void Lease();
        void OpenYard();
        void SetBid(int grade, double price);
        double ShipGrade(int grade);
    }
    public sealed partial class LocalGameService
    {
        private double CounterRate(int grade) => scrap.UnitRate((ScrapGrade)grade, 0, Prices.ScrapReference) * .86;
        private void SupplierHour(long hour)
        {
            if (!session.Yard.bandar) return;
            int grade = (int)(hour % 4);
            session.Receive(grade, CounterRate(grade), business.Positive("npcLotKg"),
                business.Positive("capacityKg"), hour);
        }
        public YardView YardInfo(int grade)
        {
            var y = session.Yard;
            return new YardView { Rep = y.reputation, Caste = y.bandar ? 2 : 1, Cash = session.Cash,
                PermitFiled = y.applied, PermitHours = Math.Max(0,y.permitHour - GameHours),
                Leased = y.leaseUntil > GameHours, LeaseHours = Math.Max(0,y.leaseUntil - GameHours),
                Stock = y.stock[grade], Bid = y.bids[grade], Floor = CounterRate(grade),
                NetShipment = scrap.UnitRate((ScrapGrade)grade,0,Prices.BuyFromK2) * y.BillableKg(grade,business.Positive("minShipmentKg"))
                    - business.Positive("shipFee"), Grade = ScrapText.Grade((ScrapGrade)grade) };
        }
        public void ApplyPermit(bool matched) => session.ApplyPermit(business, GameHours, matched);
        public void Lease() => session.Lease(business, GameHours);
        public void OpenYard() => session.OpenYard(business, GameHours);
        public void SetBid(int grade, double price) => session.SetBid(grade, price, GameHours);
        public double ShipGrade(int grade) => session.ShipGrade(grade, business,
            scrap.UnitRate((ScrapGrade)grade, 0, Prices.BuyFromK2),GameHours,YardSoil);
    }
}
