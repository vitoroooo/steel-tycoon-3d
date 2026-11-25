using Steel.Core.Trade.Town;
using Steel.Services;
using Steel.Services.Remote;

namespace Steel.Career.Careers
{
    /// <summary>What the shared city needs of a career: where its player is, and its yard's buying board.</summary>
    public sealed partial class Career
    {
        /// <summary>The check on reported positions; null lets a phone say anything, as the tests that teleport need.</summary>
        public City.MoveGuard Moves { get; private set; }
        public void Guard() => Moves = new City.MoveGuard(new CityLayout(rules), game.ClockSeconds);
        private RemoteResult Run(RemoteCall call) => Moves?.Refuses(game, call) is string why
            ? new RemoteResult { ok = false, error = why } : RemoteRunner.Run(game, call);

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
