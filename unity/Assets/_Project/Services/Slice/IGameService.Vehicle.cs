namespace Steel.Services
{
    public partial interface IGameService
    {
        /// <summary>The vehicle being driven, otherwise the one standing closest to the player.</summary>
        VehicleView Vehicle { get; }
        VehicleView VehicleOf(string id);
        void BuyVehicle(string id);
        void EnterVehicle(string id);
        void ExitVehicle(float x, float z);
        void ReportVehicle(string id, float x, float z, float speed, float? heading = null);
    }
}
