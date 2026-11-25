using System;

namespace Steel.Core.Fleet.Routes
{
    public sealed class RouteOrder
    {
        public readonly string Carrier;
        public readonly int District;
        public readonly double UnitCost;
        public readonly bool Purchased;
        public double SecondsLeft { get; private set; }
        public double KgLeft { get; private set; }

        internal RouteOrder(string carrier, int district, double seconds, double kg, double cost, bool purchased)
        {
            Carrier = carrier; District = district; SecondsLeft = seconds; KgLeft = kg;
            UnitCost = cost / kg; Purchased = purchased;
        }
        internal double Advance(double seconds)
        {
            double step = Math.Min(seconds, SecondsLeft);
            SecondsLeft -= step; return step;
        }
        internal void Take(double kg) => KgLeft -= kg;
    }
}
