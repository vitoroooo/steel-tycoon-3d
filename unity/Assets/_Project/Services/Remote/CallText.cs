using System;
using System.Globalization;
using UnityEngine;

namespace Steel.Services.Remote
{
    /// <summary>Writes a command's arguments as plain text and reads them back, the same way on the phone and on the server.</summary>
    public static class CallText
    {
        private const char Line = '\n';
        private static readonly CultureInfo Plain = CultureInfo.InvariantCulture;

        public static string Write(object value)
        {
            switch (value)
            {
                case null: return "";
                case string text: return text;
                case bool flag: return flag ? "1" : "0";
                case int whole: return whole.ToString(Plain);
                case double number: return number.ToString("R", Plain);
                case float number: return number.ToString("R", Plain);
                case string[] list: return string.Join(Line.ToString(), list);
                case Enum kind: return Convert.ToInt32(kind).ToString(Plain);
                default: return JsonUtility.ToJson(value);
            }
        }

        public static object Read(string text, Type type)
        {
            if (text == null) throw new FormatException("Argumen perintah hilang.");
            Type inner = Nullable.GetUnderlyingType(type);
            if (inner != null) return text.Length == 0 ? null : Read(text, inner);
            if (type == typeof(string)) return text;
            if (type == typeof(bool)) return text == "1" ? true : text == "0" ? false : throw Broken(text);
            if (type == typeof(int)) return int.Parse(text, NumberStyles.AllowLeadingSign, Plain);
            if (type == typeof(double)) return Finite(double.Parse(text, NumberStyles.Float, Plain));
            if (type == typeof(float)) return (float)Finite(double.Parse(text, NumberStyles.Float, Plain));
            if (type == typeof(string[])) return text.Length == 0 ? new string[0] : text.Split(Line);
            if (type.IsEnum)
            {
                object kind = Enum.ToObject(type, int.Parse(text, NumberStyles.None, Plain));
                return Enum.IsDefined(type, kind) ? kind : throw Broken(text);
            }
            if (type.IsClass) return text.Length == 0 ? null : JsonUtility.FromJson(text, type);
            throw new FormatException("Jenis argumen tidak dikenal: " + type.Name);
        }

        private static double Finite(double number) =>
            double.IsNaN(number) || double.IsInfinity(number) ? throw Broken(number.ToString(Plain)) : number;
        private static FormatException Broken(string text) => new FormatException("Argumen perintah rusak: " + text);
    }
}
