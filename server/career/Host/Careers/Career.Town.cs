using Steel.Core.Trade.Town;

namespace Steel.Career.Careers
{
    /// <summary>What the shared city needs of a career: where its player is, and its yard's buying board.</summary>
    public sealed partial class Career
    {
        public float X => game.PresenceX;
        public float Z => game.PresenceZ;
        public string Vehicle => game.PresenceVehicle;

        /// <summary>The other players' boards this career may sell to, with what each can still pay.</summary>
        public void Post(PlayerYard[] yards, double[] canPay) => game.PostYards(yards, canPay);
        /// <summary>This career's own board, or null while its yard is not trading.</summary>
        public PlayerYard Board(out double canPay) => game.Board(out canPay);
        /// <summary>The loads this player sold at other players' yards since last asked.</summary>
        public Delivery[] Deliveries() => game.TakeDeliveries();
        public void Take(Delivery note) => game.TakeDelivery(note);
    }
}
