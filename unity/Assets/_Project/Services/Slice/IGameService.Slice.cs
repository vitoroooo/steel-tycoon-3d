namespace Steel.Services
{
    public partial interface IGameService
    {
        GameView View { get; }
        InteractionView Nearby { get; }
        void SetPosition(float x, float z);
        void Inspect(string id);
        void PickUp(string id);
        void SortAtHome();
        bool IsCollected(string id);
        SaleView PreviewSale();
        double ConfirmSale();
    }
}
