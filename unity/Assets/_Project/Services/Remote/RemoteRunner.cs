using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Steel.Services.Remote
{
    /// <summary>Runs one sent call on a career: on the server's copy when a phone sends it, and on the phone's own copy
    /// when it replays what the server has not answered yet. Only IGameService methods a phone may send are known here.</summary>
    public static class RemoteRunner
    {
        private static readonly Dictionary<string, MethodInfo> Known = typeof(IGameService).GetMethods()
            .Where(method => !method.IsSpecialName && RemoteCalls.Sent(method.Name))
            .ToDictionary(method => method.Name);

        public static IEnumerable<string> Names => Known.Keys;

        public static RemoteResult Run(IGameService game, RemoteCall call)
        {
            try
            {
                if (call?.name == null || !Known.TryGetValue(call.name, out MethodInfo method))
                    return Refused("Perintah tidak dikenal.");
                ParameterInfo[] wanted = method.GetParameters();
                if ((call.args?.Length ?? 0) != wanted.Length) return Refused("Jumlah argumen perintah salah.");
                object[] values = wanted.Select((p, i) => CallText.Read(call.args[i], p.ParameterType)).ToArray();
                method.Invoke(game, values);
                return new RemoteResult { ok = true };
            }
            catch (TargetInvocationException thrown) { return Refused(thrown.InnerException?.Message ?? thrown.Message); }
            catch (Exception broken) when (broken is FormatException || broken is OverflowException || broken is ArgumentException)
            { return Refused(broken.Message); }
        }

        private static RemoteResult Refused(string why) => new RemoteResult { ok = false, error = why };
    }
}
