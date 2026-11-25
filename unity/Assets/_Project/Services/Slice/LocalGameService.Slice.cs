using System.Linq;
using Steel.Core.Economy;
using Steel.Core.State.Slice;

namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        private SliceSession session;
        private WorldSafety safety;
        public bool NeedsRespawn(double y) => safety.NeedsRespawn(y);
        private SaleOffer pending;
        private void CreateSession(IConfigSource source)
        {
            safety = new WorldSafety(source.Read("City"));
            session = new SliceSession(new SliceRules(source.Read("City")),
                new SliceWorld(source.Read("Piles")), scrap, new Core.Fleet.PickupRules(source.Read("Vehicle")));
        }
        public GameView View => new GameView
        {
            Cash = session.Cash, CarriedKg = session.Bag.CarriedKg, CapacityKg = session.Bag.CapacityKg,
            Ibn = Prices.Ibn, GameHours = GameHours, Caste = 1,
            HomeDistance = session.Position.Distance(session.Rules.Home),
            FactoryDistance = session.Position.Distance(session.Rules.Factory),
            PlayerX = (float)session.Position.X, PlayerZ = (float)session.Position.Z
        };
        public InteractionView Nearby
        {
            get
            {
                var target = session.World.Piles.Where(p => !p.Collected &&
                    p.Position.Distance(session.Position) <= session.Rules.InteractRange)
                    .OrderBy(p => p.Position.Distance(session.Position)).FirstOrDefault();
                if (target != null) return new InteractionView
                {
                    Id = target.Id, Title = "Tumpukan rongsok", Inspected = target.Scrap.KnownGrade.HasValue,
                    Detail = target.Scrap.GrossKg + " kg · " +
                        (target.Scrap.KnownGrade.HasValue ? ScrapText.Grade(target.Scrap.KnownGrade.Value) : "Grade belum diperiksa")
                };
                if (View.HomeDistance <= session.Rules.InteractRange)
                    return new InteractionView { Id = "home", Title = "Lapak Anda", Detail = "Sortir muatan sesuai grade" };
                if (View.FactoryDistance <= session.Rules.InteractRange)
                    return new InteractionView { Id = "factory", Title = "Pabrik AI", Detail = "Lihat harga & margin sebelum menjual" };
                if (Vehicle.Occupied || Vehicle.Nearby)
                    return new InteractionView { Id = "vehicle", Title = "Pikap bekas",
                        Detail = Vehicle.Occupied ? "Berhenti lalu keluar" : "Pikap pinjaman · kapasitas 1 ton" };
                return null;
            }
        }
        public void SetPosition(float x, float z) => session.Position = new WorldPoint(x, z);
        public void Inspect(string id) => session.Inspect(id);
        public void PickUp(string id) => session.PickUp(id);
        public void SortAtHome() => session.SortAtHome();
        public bool IsCollected(string id) => session.World.Find(id).Collected;
        public SaleView PreviewSale()
        {
            pending = session.Preview(Prices);
            return new SaleView { Kilograms = pending.Kilograms, Revenue = pending.Revenue, Profit = pending.Profit };
        }
        public double ConfirmSale()
        {
            double amount = session.Confirm(pending, Prices); pending = null; return amount;
        }
    }
}
