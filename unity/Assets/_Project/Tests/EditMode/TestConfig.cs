using System.Globalization;
using System.Linq;
using Steel.Core.State;
using Steel.Services;

namespace Steel.Tests
{
    internal static class TestConfig
    {
        public static string Read(string name) => new ResourceConfigSource().Read(name);

        /// <summary>The same configuration on days without cheating collectors, strikes or reuse finds, for tests that count what a yard receives.</summary>
        public static IConfigSource Calm(IConfigSource source)
        {
            foreach (string chance in new[] { "dirtyChance", "strikeChance", "reuseChance" })
                source = new OverrideConfigSource(source, "Events", chance, 1e-12);
            return source;
        }

        public static string With(string name, string key, double value)
        {
            var values = NumericJson.Parse(Read(name));
            values[key] = value;
            return "{" + string.Join(",", values.Select(pair =>
                "\"" + pair.Key + "\":" + pair.Value.ToString("R", CultureInfo.InvariantCulture))) + "}";
        }
    }
}
