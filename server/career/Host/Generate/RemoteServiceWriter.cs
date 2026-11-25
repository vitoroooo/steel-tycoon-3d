using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Steel.Services;
using Steel.Services.Remote;

namespace Steel.Career.Generate
{
    /// <summary>Writes the phone's RemoteGameService from IGameService and RemoteCalls, so a new game method cannot be
    /// forgotten: dotnet run --project server/career -- generate unity/Assets/_Project/Services/Remote/Generated.</summary>
    public static class RemoteServiceWriter
    {
        private const int PerFile = 28;

        public static Dictionary<string, string> Files()
        {
            string[] members = typeof(IGameService).GetProperties().OrderBy(p => p.Name).Select(Property)
                .Concat(typeof(IGameService).GetMethods().Where(m => !m.IsSpecialName && !RemoteCalls.Time.Contains(m.Name))
                    .OrderBy(m => m.Name).Select(Method)).ToArray();
            var files = new Dictionary<string, string>();
            for (int at = 0, part = 1; at < members.Length; at += PerFile, part++)
                files["RemoteGameService." + part + ".cs"] =
                    "// Written by server/career (generate). Do not edit: change IGameService or RemoteCalls and run it again.\n" +
                    "namespace Steel.Services.Remote\n{\n    public sealed partial class RemoteGameService\n    {\n" +
                    string.Join("\n", members.Skip(at).Take(PerFile)) + "\n    }\n}\n";
            return files;
        }

        public static void Write(string folder)
        {
            Directory.CreateDirectory(folder);
            foreach (string old in Directory.GetFiles(folder, "RemoteGameService.*.cs")) File.Delete(old);
            foreach (KeyValuePair<string, string> file in Files()) File.WriteAllText(Path.Combine(folder, file.Key), file.Value);
        }

        private static string Property(PropertyInfo property)
        {
            if (property.CanWrite) throw new NotSupportedException("IGameService." + property.Name + " has a setter.");
            return "        public " + TypeText.Of(property.PropertyType) + " " + property.Name + " => game." + property.Name + ";";
        }

        private static string Method(MethodInfo method)
        {
            string head = "        public " + TypeText.Of(method.ReturnType) + " " + method.Name + "(" + TypeText.Parameters(method) + ")";
            string call = "game." + method.Name + "(" + TypeText.Arguments(method) + ")";
            if (RemoteCalls.Queries.Contains(method.Name)) return head + " => " + call + ";";
            string report = (RemoteCalls.Motion.Contains(method.Name) ? "Moved(" : "Sent(") +
                            TypeText.Arguments(method, "\"" + method.Name + "\"") + ");";
            return method.ReturnType == typeof(void)
                ? head + " { " + call + "; " + report + " }"
                : head + " { var value = " + call + "; " + report + " return value; }";
        }
    }
}
