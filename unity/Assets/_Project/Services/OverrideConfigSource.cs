using System;
using System.Globalization;
using System.Linq;
using Steel.Core.State;

namespace Steel.Services
{
    /// <summary>Another config source with one number replaced, for tests and tuning experiments.</summary>
    public sealed class OverrideConfigSource : IConfigSource
    {
        private readonly IConfigSource inner;
        private readonly string file, key;
        private readonly double value;

        public OverrideConfigSource(IConfigSource inner, string file, string key, double value)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this.file = file; this.key = key; this.value = value;
        }

        public string Read(string name)
        {
            string json = inner.Read(name);
            if (name != file) return json;
            var values = NumericJson.Parse(json);
            if (!values.ContainsKey(key)) throw new FormatException("Missing configuration key: " + key);
            values[key] = value;
            return "{" + string.Join(",", values.Select(pair =>
                "\"" + pair.Key + "\":" + pair.Value.ToString("R", CultureInfo.InvariantCulture))) + "}";
        }
    }
}
