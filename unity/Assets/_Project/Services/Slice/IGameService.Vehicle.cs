namespace Steel.Services
{
    public partial interface IGameService
    {
        VehicleView Vehicle { get; }
        void EnterVehicle();
        void ExitVehicle(float x, float z);
        void ReportVehicle(float x, float z, float speed);
    }
}
