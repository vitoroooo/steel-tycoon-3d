using System;
using Steel.Core.Weather;

namespace Steel.Services
{
    public struct WeatherView
    {
        public string Area, Soil, Forecast;
        public bool Rain, Hangar, Paved, Stuck;
        public double CoveredKg, Settlement, DueHour;
    }
    public partial interface IGameService
    {
        WeatherView Weather(int district);
        bool RainHere { get; }
        void Shelter(bool store);
        void ImproveLand(bool pave);
        void Rescue(bool tow);
    }
    public sealed partial class LocalGameService
    {
        private long Hour => (long)Math.Floor(GameHours);
        private int YardSoil => DistrictWeather.Soil(Hour,DistrictWeather.District(session.Rules.Home));
        public bool RainHere => DistrictWeather.Raining(Hour,DistrictWeather.District(session.Position));
        public WeatherView Weather(int district)
        {
            var y = session.Yard; int soil = DistrictWeather.Soil(Hour,district);
            return new WeatherView { Area = Areas[district], Rain = DistrictWeather.Raining(Hour,district),
                Soil = new[] {"kering","becek","lumpur"}[soil], Hangar = y.hangar, Paved = y.paved,
                CoveredKg = session.Covered.CarriedKg, Stuck = y.stuck, Settlement = y.payout, DueHour = y.unloadHour,
                Forecast = DistrictWeather.RainHours(Hour+1,district) + " dari 24 jam ke depan diprakirakan hujan" };
        }
        public void Shelter(bool store) { session.MoveCovered(store); if (store) StoredUnderRoof = true; }
        public void ImproveLand(bool pave) => session.InstallRoof(pave,business.Positive(pave ? "pave" : "roof"));
        public void Rescue(bool tow) => session.CrewRescue(tow,GameHours,business.Positive("tow"),
            session.Yard.Machine(true,GameHours).condition,session.Caste);
    }
}
