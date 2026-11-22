namespace Steel.Presentation.UI
{
    public sealed partial class CityHud
    {
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
                return "Dipungut " + (host.Game.View.CarriedKg - before) + " kg, muatan penuh. Sisanya menunggu di gang.";
            return host.Game.Income.StreetSites > 0 ? "Rongsok dipungut. Sortir di lapak Anda." :
                "Tumpukan terakhir di gang dipungut. Sortir di lapak, lalu baca pesan Pak Darto.";
        }
    }
}
