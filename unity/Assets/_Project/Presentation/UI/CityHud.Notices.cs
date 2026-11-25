using Steel.Services;

namespace Steel.Presentation.UI
{
    public sealed partial class CityHud
    {
        private DoorDeal doors;
        private HaulDesk haul;
        /// <summary>Places that open their own screen: a door that sells scrap, the yard's loading post, the mill's weighing post.</summary>
        private bool ActPost(InteractionView near)
        {
            if (near.Id == "seller") (doors ??= new DoorDeal(this, host.Game, input)).Open(host.Game.NearSeller.Id);
            else if (near.Id == "gate" || near.Id == "yardpost") (haul ??= new HaulDesk(this, host.Game, input)).Open(near.Id == "gate");
            else if (near.Id == "playeryard") NoticeText.text = SellAtYard(host.Game.View);
            else return false;
            return true;
        }

        /// <summary>Sells the carried load at the other player's board the player stands at (PRD 5.2).</summary>
        private string SellAtYard(GameView me)
        {
            PlayerYardView yard = System.Linq.Enumerable.First(System.Linq.Enumerable.OrderBy(host.Game.PlayerYards,
                y => (y.X - me.PlayerX) * (y.X - me.PlayerX) + (y.Z - me.PlayerZ) * (y.Z - me.PlayerZ)));
            return "Disetor ke lapak " + yard.Owner + ": kas bertambah " + Money(host.Game.SellToPlayerYard(yard.Plot));
        }

        private string lastAlarm;
        /// <summary>Trouble at the yard is announced wherever the player is.</summary>
        private void Alarm()
        {
            string note = host.Game.Risk.Note;
            if (note != lastAlarm && (lastAlarm = note) != null) NoticeText.text = note;
        }

        private void ToggleCart()
        {
            if (host.Game.Cart.Held) host.Game.ReleaseCart(); else host.Game.GrabCart();
            NoticeText.text = host.Game.Cart.Held ? "Gerobak didorong. Muatan sampai 50 kg." : "Gerobak diparkir.";
        }

        /// <summary>Picking up the last street site must not read like the end of the income.</summary>
        private string PickUp(string id)
        {
            double before = host.Game.View.CarriedKg;
            host.Game.PickUp(id);
            if (!host.Game.IsCollected(id))
                return "Terangkut " + (host.Game.View.CarriedKg - before) + " kg, penuh. Sisanya ditinggal dulu.";
            return host.Game.Income.StreetSites > 0 ? "Rongsok dipungut. Sortir di lapak Anda." :
                "Itu tumpukan terakhir di gang. Sortir di lapak, lalu baca pesan Pak Darto.";
        }
    }
}
