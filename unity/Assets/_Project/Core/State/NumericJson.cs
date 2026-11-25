using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Steel.Core.State
{
    public static class NumericJson
    {
        private static readonly Regex Pair = new Regex(
            @"\G\s*""([A-Za-z][A-Za-z0-9_]*)""\s*:\s*(-?(?:0|[1-9][0-9]*)(?:\.[0-9]+)?(?:[eE][+-]?[0-9]+)?)",
            RegexOptions.CultureInvariant);

        public static Dictionary<string, double> Parse(string json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));
            string body = json.Trim();
            if (body.Length < 2 || body[0] != '{' || body[body.Length - 1] != '}')
                throw new FormatException("Expected a flat numeric JSON object.");
            body = body.Substring(1, body.Length - 2);
            var result = new Dictionary<string, double>(StringComparer.Ordinal);
            int position = 0;
            if (string.IsNullOrWhiteSpace(body)) return result;
            while (position < body.Length)
            {
                Match pair = Pair.Match(body, position);
                if (!pair.Success || pair.Index != position)
                    throw new FormatException("Expected a numeric configuration entry.");
                string key = pair.Groups[1].Value;
                if (result.ContainsKey(key)) throw new FormatException("Duplicate key: " + key);
                if (!double.TryParse(pair.Groups[2].Value, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out double value) ||
                    double.IsNaN(value) || double.IsInfinity(value))
                    throw new FormatException("Non-finite configuration value: " + key);
                result.Add(key, value);
                position = pair.Index + pair.Length;
                while (position < body.Length && char.IsWhiteSpace(body[position])) position++;
                if (position == body.Length) return result;
                if (body[position++] != ',' || string.IsNullOrWhiteSpace(body.Substring(position)))
                    throw new FormatException("Expected another entry after comma.");
            }
            throw new FormatException("Incomplete configuration.");
        }
    }
}
