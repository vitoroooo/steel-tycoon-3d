namespace Steel.Services
{
    public partial interface IGameService
    {
        CartView Cart { get; }
        void GrabCart();
        void ReleaseCart();
    }
}
