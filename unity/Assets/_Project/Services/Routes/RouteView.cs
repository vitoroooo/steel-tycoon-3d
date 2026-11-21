namespace Steel.Services
{
    public sealed class RouteView
    {
        public string Carrier, Area, NextCarrier;
        public int District, Queue;
        public bool Purchased;
        public double Kg, Minutes, Goods, Travel, DepotKg, ArrivalMinutes;
    }

    public partial interface IGameService
    {
        RouteView Route(string carrier, int district, bool purchased);
        void BookRoute(RouteView preview);
        double CollectRoute();
    }
}
