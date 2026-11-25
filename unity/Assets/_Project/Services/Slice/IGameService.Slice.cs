namespace Steel.Services
{
    public partial interface IGameService
    {
        GameView View { get; }
        bool NeedsRespawn(double y);
        InteractionView Nearby { get; }
        void SetPosition(float x, float z);
        void Inspect(string id);
        void PickUp(string id);
        void SortAtHome();
        bool IsCollected(string id);
        SaleView PreviewSale();
        double ConfirmSale();
        double ClaimDealerGrant();
    }
}
