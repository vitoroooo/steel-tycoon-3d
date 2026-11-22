using System;
using System.Linq;
using System.Reflection;

namespace Steel.Career.Generate
{
    /// <summary>C# source text for the types in IGameService's signatures.</summary>
    public static class TypeText
    {
        public static string Of(Type type)
        {
            if (type == typeof(void)) return "void";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(int)) return "int";
            if (type == typeof(float)) return "float";
            if (type == typeof(double)) return "double";
            if (type == typeof(string)) return "string";
            if (type.IsArray) return Of(type.GetElementType()) + "[]";
            Type inner = Nullable.GetUnderlyingType(type);
            return inner != null ? Of(inner) + "?" : "global::" + type.FullName.Replace('+', '.');
        }

        public static string Parameters(MethodInfo method) =>
            string.Join(", ", method.GetParameters().Select(p => Of(p.ParameterType) + " " + p.Name));

        public static string Arguments(MethodInfo method, string first = null) =>
            string.Join(", ", (first == null ? new string[0] : new[] { first }).Concat(method.GetParameters().Select(p => p.Name)));
    }
}
