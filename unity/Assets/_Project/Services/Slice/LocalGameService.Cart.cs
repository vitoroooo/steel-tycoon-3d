namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        public CartView Cart
        {
            get
            {
                var cart = session.Cart;
                return new CartView
                {
                    Held = cart.Held, Nearby = !session.Pickup.Occupied && cart.InReach(session.Position),
                    X = (float)cart.Position.X, Z = (float)cart.Position.Z,
                    PushSpeed = (float)cart.Rules.PushSpeed,
                    CargoKg = cart.Cargo.CarriedKg, CapacityKg = cart.Rules.CapacityKg
                };
            }
        }

        public void GrabCart() { session.GrabCart(); pending = null; }
        public void ReleaseCart() { session.ReleaseCart(); pending = null; }
    }
}
