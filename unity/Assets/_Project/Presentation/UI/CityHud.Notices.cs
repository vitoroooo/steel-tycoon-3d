namespace Steel.Presentation.UI
{
    public sealed partial class CityHud
    {
        private DoorDeal doors;
        /// <summary>The dealing screen is only built the first time the player stops at a door.</summary>
        private void OpenDoor() => (doors ??= new DoorDeal(this, host.Game, input)).Open(host.Game.NearSeller.Id);

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
