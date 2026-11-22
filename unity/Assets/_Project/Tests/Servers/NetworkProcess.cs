using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Steel.TestServers
{
    public sealed class NetworkProcess : IDisposable
    {
        private readonly string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        private readonly string root = Path.GetFullPath(Path.Combine(Application.dataPath, "../.."));
        private Process server;
        public int Port { get; private set; }
        public string Root => root;
        public string Database => Path.Combine(directory, "market.sqlite3");

        private Process Launch(string command)
        {
            string database = Path.Combine(directory, "market.sqlite3");
            var start = new ProcessStartInfo("python3", "-m server.market --db \"" + database + "\" " + command)
            {
                WorkingDirectory = root, RedirectStandardOutput = true, UseShellExecute = false,
                CreateNoWindow = true
            };
            return Process.Start(start);
        }

        public void Start()
        {
            if (!File.Exists(Path.Combine(root, "server/market/__main__.py")))
                Assert.Ignore("Uji integrasi pasar memerlukan checkout lengkap termasuk server Python.");
            Directory.CreateDirectory(directory);
            try { server = Launch("serve --port 0"); }
            catch (Win32Exception error) when (error.NativeErrorCode == 2)
            { Assert.Ignore("Uji integrasi pasar memerlukan python3 pada PATH editor."); }
            var ready = server.StandardOutput.ReadLineAsync();
            Assert.IsTrue(ready.Wait(10000), "Server tidak siap dalam 10 detik.");
            Assert.IsNotNull(ready.Result, "Server lokal gagal memulai.");
            Port = int.Parse(ready.Result.Substring(ready.Result.LastIndexOf(':') + 1));
        }

        public string IssueToken(bool seller)
        {
            using (var process = Launch(seller ? "trader Sari" : "trader Hasan"))
            {
                if (!process.WaitForExit(10000))
                {
                    process.Kill();
                    Assert.Fail("Penerbitan token melewati batas waktu.");
                }
                Assert.AreEqual(0, process.ExitCode, "Penerbitan token gagal.");
                return process.StandardOutput.ReadToEnd().Trim();
            }
        }

        public void Dispose()
        {
            if (server != null)
            {
                if (!server.HasExited) server.Kill();
                server.WaitForExit(5000);
                server.Dispose();
            }
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }
}
