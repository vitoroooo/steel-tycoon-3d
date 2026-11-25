namespace Steel.Services
{
    /// <summary>The server's side of the single purse. Not part of IGameService, so no phone can call it.</summary>
    public sealed partial class LocalGameService
    {
        public double WalletCash => session.Cash;
        public double WalletStockKg => session.MarketStockKg;
        public int WalletCaste => session.Caste;
        public void SettleMarket(double cash, double kg) => session.SettleMarket(cash, kg);
    }
}
