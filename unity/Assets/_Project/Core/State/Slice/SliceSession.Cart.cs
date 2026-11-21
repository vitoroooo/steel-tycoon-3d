using System;
using Steel.Core.Fleet;

namespace Steel.Core.State.Slice
{
    public sealed partial class SliceSession
    {
        public CartState Cart { get; private set; }

        /// <summary>Takes the push bar; anything carried by hand is dropped into the cart.</summary>
        public void GrabCart()
        {
            if (Routes.Busy("cart")) throw new InvalidOperationException("Gerobak sedang menjalankan rute.");
            if (Fleet.Driven != null) throw new InvalidOperationException("Turun dari kendaraan dahulu.");
            if (Cart.Cargo.CarriedKg + backpack.CarriedKg > Cart.Rules.CapacityKg)
                throw new InvalidOperationException("Gerobak penuh. Jual muatan dahulu.");
            Cart.Grab(Position);
            foreach (var pile in backpack.Items) Cart.Cargo.Add(pile);
            backpack.Clear();
        }

        public void ReleaseCart() => Cart.Release();
    }
}
