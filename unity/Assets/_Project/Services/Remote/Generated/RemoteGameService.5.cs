// Written by server/career (generate). Do not edit: change IGameService or RemoteCalls and run it again.
namespace Steel.Services.Remote
{
    public sealed partial class RemoteGameService
    {
        public void SortAtHome() { game.SortAtHome(); Sent("SortAtHome"); }
        public string StartWithLegacies(string[] picks) { var value = game.StartWithLegacies(picks); Sent("StartWithLegacies", picks); return value; }
        public void StopLoading() { game.StopLoading(); Sent("StopLoading"); }
        public string TakeFlashTender() { var value = game.TakeFlashTender(); Sent("TakeFlashTender"); return value; }
        public void TakeSpk(int slot) { game.TakeSpk(slot); Sent("TakeSpk", slot); }
        public void ToggleAutoDefend() { game.ToggleAutoDefend(); Sent("ToggleAutoDefend"); }
        public void ToggleCrewPremium() { game.ToggleCrewPremium(); Sent("ToggleCrewPremium"); }
        public global::Steel.Services.VehicleView VehicleOf(string id) => game.VehicleOf(id);
        public global::Steel.Services.WeatherView Weather(int district) => game.Weather(district);
        public double WeighIn() { var value = game.WeighIn(); Sent("WeighIn"); return value; }
        public void WeighSeller(string id) { game.WeighSeller(id); Sent("WeighSeller", id); }
        public global::Steel.Services.YardView YardInfo(int grade) => game.YardInfo(grade);
    }
}
