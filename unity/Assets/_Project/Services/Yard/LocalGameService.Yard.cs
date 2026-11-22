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
            // a collectors' strike halves what arrives; a dirty load waits on the scale for the owner's answer
            double kg = business.Positive("npcLotKg") * session.EventTerms.Supply(Core.Trade.Spk.SpkRules.Day(hour));
            if (session.HoldDeposit(grade, kg, CounterRate(grade), business.Positive("capacityKg"), hour)) return;
            double before = session.Yard.stock[grade];
            session.CrewReceive(grade, CounterRate(grade), kg,
                business.Positive("capacityKg"), hour, session.Yard.Machine(false,hour).condition, session.Caste);
            session.SortDeposit(grade, session.Yard.stock[grade] - before, hour);
        }
        public YardView YardInfo(int grade)
        {
            var y = session.Yard;
            return new YardView { Rep = y.reputation, Caste = session.Caste, Cash = session.Cash,
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
        public double ShipGrade(int grade) => session.CrewShipGrade(grade, business,
            scrap.UnitRate((ScrapGrade)grade, 0, Prices.BuyFromK2),GameHours,YardSoil,
            session.Yard.Machine(true,GameHours).condition,session.Caste);
    }
}
