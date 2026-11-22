namespace Steel.Services
{
    public struct CrewView
    {
        public bool Open, Senior, Premium, Protest;
        public string Policy, Shift, Stop;
        public double DayStamina, NightStamina, Morale, Patience, Wage, LastFine, CsrCost, Training;
        public double InvestigationLeft, BlockadeLeft, CsrWait, OfflineHours;
        public int Incidents;
    }

    public partial interface IGameService
    {
        CrewView CrewInfo();
        void CycleCrewPolicy();
        void ToggleCrewPremium();
        void PromoteCrewForeman();
        void FundCrewCsr();
    }
}
