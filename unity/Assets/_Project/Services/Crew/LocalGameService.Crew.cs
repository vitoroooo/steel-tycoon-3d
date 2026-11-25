using System;
using Steel.Core.Crew;
using Steel.Core.State;

namespace Steel.Services
{
    public sealed partial class LocalGameService
    {
        private ConfigValues crewConfig;
        private void InitCrew(IConfigSource source)
        {
            crewConfig = new ConfigValues(source.Read("Crew"));
            session.InitCrew(crewConfig);
        }

        public CrewView CrewInfo()
        {
            var c = session.Crew;
            double hour = GameHours;
            return new CrewView {
                Open = session.Yard.bandar, Senior = c.senior, Premium = c.premium, Policy = c.policy.ToString(),
                Shift = CrewSave.IsDay(hour) ? "siang" : "malam", Stop = session.CrewStopReason(hour, 100),
                DayStamina = c.dayStamina, NightStamina = c.nightStamina, Morale = c.morale,
                Patience = c.patience, Wage = session.CrewWage, LastFine = c.lastFine, Incidents = c.incidents,
                Protest = c.patience < crewConfig.Get("protestThreshold"),
                InvestigationLeft = Math.Max(0, c.investigateUntil - hour),
                BlockadeLeft = Math.Max(0, c.blockadeUntil - hour), CsrWait = Math.Max(0, c.csrReady - hour),
                CsrCost = session.CsrCost, Training = crewConfig.Get("trainingCost"),
                OfflineHours = session.CrewQueueLimit(queueLimit) / 3600
            };
        }
        public void CycleCrewPolicy() => session.SetCrewPolicy((ShiftPolicy)(((int)session.Crew.policy + 1) % 3));
        public void ToggleCrewPremium() => session.SetCrewPremium(!session.Crew.premium);
        public void PromoteCrewForeman() => session.PromoteForeman();
        public void FundCrewCsr() => session.FundCrewCsr(GameHours);
    }
}
