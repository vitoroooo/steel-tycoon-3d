using System;
using Steel.Core.State.Slice;
using Steel.Core.Trade.Street;

namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        private StreetLayout doors;

        private void CreateStreet(IConfigSource source)
        {
            doors = new StreetLayout(source);
            session.OpenStreet(new StreetBlock(source.Read("StreetTrade"), source.Read("Sellers")));
        }

        public int SellerCount => session.Street.Sellers.Count;

        public SellerView NearSeller
        {
            get
            {
                var seller = session.Street.Near(session.Position, session.Street.Rules.Reach);
                return seller == null ? null : Seller(seller.Id);
            }
        }

        public SellerView Seller(string id)
        {
            var s = session.Street.Find(id);
            double room = session.Bag.CapacityKg - session.Bag.CarriedKg;
            var view = new SellerView
            {
                Id = s.Id, Kind = (int)s.Kind, Title = StreetText.Title(s.Kind), Place = doors.Place(s.Id),
                X = (float)s.Position.X, Z = (float)s.Position.Z, Stocked = s.Stock != null,
                Inspected = s.Stock?.KnownGrade.HasValue ?? false, Weighed = s.Weighed, Agreed = s.Agreed,
                Grade = s.Stock?.KnownGrade.HasValue ?? false ? ScrapText.Grade(s.Stock.KnownGrade.Value) : "Belum dilihat",
                StockKg = s.ClaimedKg, AskPerKg = SliceSession.PerKg(s, Prices.BuyFromK1), Step = session.Street.Rules.Step,
                Cash = session.Cash, RoomKg = room,
                RestockMinutes = session.Street.HoursToRestock(GameHours) * hourSeconds / 60
            };
            if (s.Stock == null || room <= 0) return view;
            view.TakenKg = Math.Min(s.Stock.GrossKg, room); view.ChargedKg = s.Charged(view.TakenKg);
            view.Total = Math.Ceiling(view.ChargedKg * view.AskPerKg);
            return view;
        }

        public bool SellerStocked(string id) => session.Street.Find(id).Stock != null;
        public void InspectSeller(string id) => session.InspectSeller(id);
        public void WeighSeller(string id) => session.WeighSeller(id);

        public HaggleView Haggle(string id, double perKg)
        {
            var reply = session.Haggle(id, perKg, Prices.BuyFromK1);
            return new HaggleView { Deal = reply == BargainReply.Deal,
                Line = StreetText.Reply(reply, SliceSession.PerKg(session.Street.Find(id), Prices.BuyFromK1)) };
        }

        public double BuyAtDoor(string id) => session.BuyAtDoor(id, Prices.BuyFromK1);
    }
}
