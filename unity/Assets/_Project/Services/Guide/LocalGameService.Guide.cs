using System;
using Steel.Core.State.Saving;

namespace Steel.Services
{
    public partial interface IGameService
    {
        int Story { get; }
        void ReadStory();
        void ReadForecast();
        bool MotorRoute { get; }
        bool ReadWeather { get; }
        bool StoredUnderRoof { get; }
        double NextBikeMinutes { get; }
        bool HasSteel { get; }
    }
    public sealed partial class LocalGameService
    {
        public int Story { get; private set; }
        public bool MotorRoute { get; private set; }
        public bool ReadWeather { get; private set; }
        public bool StoredUnderRoof { get; private set; }
        public void ReadStory() => Story = Math.Min(2,Story+1);
        public void ReadForecast() => ReadWeather = true;
        public bool HasSteel
        {
            get
            {
                foreach (var lot in session.Bag.Items)
                    if (lot.KnownGrade != Core.Scrap.ScrapGrade.Reuse) return true;
                return false;
            }
        }
        public double NextBikeMinutes
        {
            get
            {
                foreach (var r in session.Routes.Orders)
                    if (r.Carrier == FleetCatalog.Triseda) return Math.Ceiling(r.SecondsLeft/60);
                return 0;
            }
        }
        private void SaveGuide(CareerSave s)
        {
            s.story = Story; s.motorRoute = MotorRoute; s.readForecast = ReadWeather;
            s.sheltered = StoredUnderRoof; s.streetTip = StreetTipRead;
        }
        private void LoadGuide(CareerSave s)
        {
            Story = s.firstSale ? 2 : Math.Max(0,Math.Min(2,s.story)); MotorRoute = s.motorRoute;
            ReadWeather = s.readForecast; StoredUnderRoof = s.sheltered; StreetTipRead = s.streetTip;
        }
    }
}
