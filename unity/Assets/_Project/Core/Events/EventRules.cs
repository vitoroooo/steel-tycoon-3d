using Steel.Core.State;

namespace Steel.Core.Events
{
    /// <summary>Numbers for the city events of PRD appendix E that have no rules of their own elsewhere, read from Events.json.</summary>
    public sealed class EventRules
    {
        private readonly ConfigValues values;
        public EventRules(string json) { values = new ConfigValues(json); }
        public double Get(string key) => values.Positive(key);

        /// <summary>"Pengepul AI mogok": a city event, so every yard sees the strike on the same game days.</summary>
        public bool CollectorStrike(long day) => Dice.Unit(unchecked((uint)day * 2654435761u + 97u)) < Get("strikeChance");

        /// <summary>How much of their usual load AI collectors bring on this game day.</summary>
        public double Supply(long day) => CollectorStrike(day) ? Get("strikeSupply") : 1;
    }
}
