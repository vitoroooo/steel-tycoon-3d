using System;
using Steel.Core.Scrap;
using Steel.Core.State.Slice;

namespace Steel.Core.Fleet
{
    /// <summary>The gerobak: it carries the load only while the player holds its push bar.</summary>
    public sealed class CartState
    {
        public CartRules Rules { get; }
        public Backpack Cargo { get; }
        public WorldPoint Position { get; private set; }
        public bool Held { get; private set; }

        public CartState(CartRules rules)
        {
            Rules = rules;
            Cargo = new Backpack(rules.CapacityKg);
            Position = rules.Spawn;
        }

        public bool InReach(WorldPoint player) => player.Distance(Position) <= Rules.GrabRange;

        public void Grab(WorldPoint player)
        {
            if (Held) throw new InvalidOperationException("Gerobak sudah dipegang.");
            if (!InReach(player)) throw new InvalidOperationException("Dekati gagang gerobak terlebih dahulu.");
            Held = true;
        }

        public void Release()
        {
            if (!Held) throw new InvalidOperationException("Gerobak belum dipegang.");
            Held = false;
        }

        /// <summary>While held the cart travels with the player; a parked cart never moves.</summary>
        public void Follow(WorldPoint player)
        {
            if (Held) Position = player;
        }
    }
}
