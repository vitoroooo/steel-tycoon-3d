using System;

namespace Steel.Core.State.Saving
{
    [Serializable] public sealed class CareerSave
    {
        public int version;
        public long savedUtc;
        public double seconds, cash, x, z, cartX, cartZ;
        public bool firstSale, grant, cartHeld;
        public LotSave[] bag, cart;
        public SiteSave[] sites;
        public MotorSave[] motors;
        public OrderSave[] routes;
        public double[] quota;
    }
    [Serializable] public sealed class LotSave
    {
        public int grade;
        public double kg, dirt, quality, cost;
        public bool inspected, sorted;
    }
    [Serializable] public sealed class SiteSave
    {
        public string id;
        public bool inspected, collected;
    }
    [Serializable] public sealed class MotorSave
    {
        public string id;
        public bool owned, occupied;
        public double x, z, seconds;
        public LotSave[] cargo;
    }
    [Serializable] public sealed class OrderSave
    {
        public string id;
        public int district;
        public bool bought;
        public double remaining, kg, unitCost;
    }
}
