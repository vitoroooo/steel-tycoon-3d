using System.Globalization;
using System.Linq;
using Steel.Core.Events;
using Steel.Core.State;
using Steel.Tests.Risk;

namespace Steel.Tests.Events
{
    /// <summary>The rich Kasta 2 yard of the risk tests, with Events.json and a scale that collectors' loads can be weighed on.</summary>
    public abstract class EventFixture : RiskFixture
    {
        protected const double Capacity = 60000, Floor = 3000;

        protected static EventRules Events(params (string key, double value)[] changes)
        {
            var values = NumericJson.Parse(TestConfig.Read("Events"));
            foreach (var change in changes) values[change.key] = change.value;
            return new EventRules("{" + string.Join(",", values.Select(pair =>
                "\"" + pair.Key + "\":" + pair.Value.ToString("R", CultureInfo.InvariantCulture))) + "}");
        }

        protected void OpenYard(params (string key, double value)[] changes)
        {
            Open(Rules());
            Game.OpenEvents(Events(changes));
            Game.ConfigureMaintenance(new Steel.Core.Equipment.MaintenanceRules(TestConfig.Read("Maintenance"),
                new ConfigValues(TestConfig.Read("Business"))));
            Game.Yard.leaseUntil = 1e9;
            for (int grade = 0; grade < 4; grade++) Game.Yard.bids[grade] = 4000;
        }

        /// <summary>Crew.json with these numbers changed; it holds the newcomer's protection and the two-events-a-day window.</summary>
        protected void Crew(params (string key, double value)[] changes)
        {
            var values = NumericJson.Parse(TestConfig.Read("Crew"));
            foreach (var change in changes) values[change.key] = change.value;
            Game.InitCrew(new ConfigValues("{" + string.Join(",", values.Select(pair =>
                "\"" + pair.Key + "\":" + pair.Value.ToString("R", CultureInfo.InvariantCulture))) + "}"));
        }

        /// <summary>A Kasta 3 mill standing at its own gate, with billet in the warehouse.</summary>
        protected void OpenMill(params (string key, double value)[] changes)
        {
            OpenYard(changes);
            Game.Mill.filed = Game.Mill.opened = true;
            Game.Mill.goods[0] = 20000;
            Game.Position = Game.Rules.Factory;
        }
    }
}
