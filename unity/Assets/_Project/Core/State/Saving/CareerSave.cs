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
        public SellerSave[] sellers;
        public Steel.Core.Trade.Haul.TruckLoad[] loads;
        public Steel.Core.Trade.Spk.SpkSave spk;
        public Steel.Core.Risk.RiskSave risk;
        public Steel.Core.Events.EventSave events;
        public MotorSave[] motors;
        public OrderSave[] routes;
        public double[] quota;
        public bool magnet, xrf;
        public YardSave yard;
        public MillSave mill;
        public TenderSave tender;
        public SatelliteSave[] satellites;
        public FinanceSave finance;
        public Steel.Core.Crew.CrewSave crew;
        public LotSave[] covered;
        public int story;
        public bool motorRoute, readForecast, sheltered, streetTip, guideRead;
    }
    [Serializable] public sealed class SellerSave
    {
        public string id;
        public long round;
        public double kg, claimed, ask;
        public int patience;
        public bool weighed, agreed, inspected;
    }
    [Serializable] public sealed class LotSave
    {
        public int grade;
        public int rainHours;
        public double kg, dirt, quality, cost;
        public bool inspected, sorted;
    }
    [Serializable] public sealed class SiteSave
    {
        public string id;
        public bool inspected, collected;
        public int rainHours;
        public long emptied;
        public double kg;
    }
    [Serializable] public sealed class MotorSave
    {
        public string id;
        public bool owned, occupied, hasYaw;
        public double x, z, seconds, yaw;
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
