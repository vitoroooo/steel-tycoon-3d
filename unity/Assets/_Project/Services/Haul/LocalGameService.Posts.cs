namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        /// <summary>The mill's weighing post, or the loading post of the player's own yard, when the player stands at one.</summary>
        private InteractionView NearbyPost()
        {
            if (session.Position.Distance(session.Haul.Gate) <= session.Haul.DeskReach)
                return new InteractionView { Id = "gate", Title = "Pos timbang PT Baja Nusantara", Detail = "Timbang dan bongkar muatan truk" };
            if (session.Yard.bandar && session.Position.Distance(session.Haul.Yard) <= session.Haul.DeskReach)
                return new InteractionView { Id = "yardpost", Title = "Pos muat kavling Anda", Detail = "Muat stok lapak ke truk dengan loader" };
            return null;
        }
    }
}
