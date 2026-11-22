using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using Steel.Tests.Network;

namespace Steel.Tests.Remote
{
    /// <summary>The real career server (server/career, built beforehand) beside the real market server, for one test.</summary>
    internal sealed class CareerProcess : IDisposable
    {
        private readonly NetworkProcess market = new NetworkProcess();
        private Process server;
        internal int Port { get; private set; }
        /// <summary>An account the market issued; the career server knows it by the same token.</summary>
        internal string Token { get; private set; }

        internal void Start()
        {
            market.Start();
            string folder = Path.Combine(market.Root, "server/career/bin"), resources =
                "\"" + market.Database + "\" \"" + Path.Combine(market.Root, "unity/Assets/_Project/Resources") + "\" 0";
            string alone = Path.Combine(folder, "selfhost/Steel.Career"), library = Path.Combine(folder, "Debug/net8.0/Steel.Career.dll");
            if (!File.Exists(alone) && !File.Exists(library))
                Assert.Ignore("Uji server karier memerlukan server/career yang sudah dibangun (lihat server/career/README.md).");
            Token = market.IssueToken(true);   // the market creates the database the career server opens
            var start = File.Exists(alone) ? new ProcessStartInfo(alone, resources)
                                           : new ProcessStartInfo(Dotnet(), "\"" + library + "\" " + resources);
            start.RedirectStandardOutput = true; start.UseShellExecute = false; start.CreateNoWindow = true;
            start.EnvironmentVariables["DOTNET_SYSTEM_GLOBALIZATION_INVARIANT"] = "1";
            try { server = Process.Start(start); }
            catch (Win32Exception) { Assert.Ignore("Uji server karier memerlukan dotnet 8."); }
            var ready = server.StandardOutput.ReadLineAsync();
            Assert.IsTrue(ready.Wait(20000) && ready.Result != null, "Server karier tidak siap dalam 20 detik.");
            Port = int.Parse(ready.Result.Substring(ready.Result.LastIndexOf(':') + 1));
        }


        private static string Dotnet()
        {
            string home = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "dotnet", "dotnet");
            return File.Exists(home) ? home : "dotnet";
        }

        public void Dispose()
        {
            if (server != null)
            {
                if (!server.HasExited) server.Kill();
                server.WaitForExit(5000); server.Dispose();
            }
            market.Dispose();
        }
    }
}
