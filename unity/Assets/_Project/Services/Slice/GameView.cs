namespace Steel.Services
{
    public sealed class GameView
    {
        public double Cash, CarriedKg, CapacityKg, Ibn, GameHours, HomeDistance, FactoryDistance;
        public int Caste;
    }
    public sealed class InteractionView
    {
        public string Id, Title, Detail;
        public bool Inspected;
    }
    public sealed class SaleView
    {
        public double Kilograms, Revenue, Profit;
    }
}
