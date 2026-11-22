using System.Globalization;
using System.Linq;
using Steel.Core.State;
using Steel.Services;

namespace Steel.Tests
{
    internal static class TestConfig
    {
        public static string Read(string name) => new ResourceConfigSource().Read(name);

        /// <summary>The same configuration on days without cheating collectors or strikes, for tests that count what a yard receives.</summary>
        public static IConfigSource Calm(IConfigSource source) => new OverrideConfigSource(
            new OverrideConfigSource(source, "Events", "dirtyChance", 1e-12), "Events", "strikeChance", 1e-12);

        public static string With(string name, string key, double value)
        {
            var values = NumericJson.Parse(Read(name));
            values[key] = value;
            return "{" + string.Join(",", values.Select(pair =>
                "\"" + pair.Key + "\":" + pair.Value.ToString("R", CultureInfo.InvariantCulture))) + "}";
        }
    }
}
