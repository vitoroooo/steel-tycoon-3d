using System.Globalization;
using System.Linq;
using Steel.Core.State;
using Steel.Services;

namespace Steel.Tests
{
    internal static class TestConfig
    {
        public static string Read(string name) => new ResourceConfigSource().Read(name);

        public static string With(string name, string key, double value)
        {
            var values = NumericJson.Parse(Read(name));
            values[key] = value;
            return "{" + string.Join(",", values.Select(pair =>
                "\"" + pair.Key + "\":" + pair.Value.ToString("R", CultureInfo.InvariantCulture))) + "}";
        }
    }
}
