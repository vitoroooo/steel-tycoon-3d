namespace Steel.Core.Fleet.Routes
{
    public sealed class RouteTicket
    {
        public readonly string Carrier;
        public readonly int District;
        public readonly bool Purchased;
        public readonly double Kg, Seconds, Goods, Travel;
        public double Cost => Goods + Travel;

        public RouteTicket(string carrier, int district, bool purchased, double kg, double seconds,
            double goods, double travel)
        {
            Carrier = carrier; District = district; Purchased = purchased;
            Kg = kg; Seconds = seconds; Goods = goods; Travel = travel;
        }
    }
}
