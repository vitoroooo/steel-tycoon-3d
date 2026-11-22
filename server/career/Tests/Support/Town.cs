using System;
using System.Diagnostics;
using System.IO;
using Steel.Career.Accounts;
using Steel.Career.Careers;
using Steel.Services;

namespace Steel.Career.Tests.Support
{
    /// <summary>A career server over a fresh market database, with accounts issued by the market service itself
    /// and a clock the test moves by hand.</summary>
    public sealed class Town : IDisposable
    {
        private static readonly string Repo = FindRepo();
        public static readonly string Resources = Path.Combine(Repo, "unity", "Assets", "_Project", "Resources");

        public readonly string Database = Path.Combine(Path.GetTempPath(), "career-" + Guid.NewGuid().ToString("N") + ".db");
        public readonly AccountBook Accounts;
        public readonly CareerBook Careers;
        public readonly IConfigSource Rules = new ResourceConfigSource();
        public long NowMs = 1_800_000_000_000;

        /// <summary>A guarded town checks reported positions as the public server does; the others let a test teleport.</summary>
        public Town(bool guarded = false)
        {
            UnityEngine.Resources.Root = Resources;
            Market("trader", "Pembuka");
            Accounts = new AccountBook(Database);
            Careers = new CareerBook(Rules, new CareerStore(Accounts), () => NowMs) { Roaming = !guarded };
        }

        public void Pass(double seconds) => NowMs += (long)Math.Round(seconds * 1000);

        /// <summary>Opens an account the way an operator does and returns its token.</summary>
        public string Token(string name, bool rotate = true)
        {
            if (!rotate) return tokens[name];
            return tokens[name] = Market("trader", name).Trim();
        }
        private readonly System.Collections.Generic.Dictionary<string, string> tokens =
            new System.Collections.Generic.Dictionary<string, string>();

        public string Market(params string[] words)
        {
            var start = new ProcessStartInfo("python3") { WorkingDirectory = Repo, RedirectStandardOutput = true, RedirectStandardError = true };
            foreach (string word in new[] { "-m", "server.market", "--db", Database }) start.ArgumentList.Add(word);
            foreach (string word in words) start.ArgumentList.Add(word);
            using Process run = Process.Start(start);
            string output = run.StandardOutput.ReadToEnd(), errors = run.StandardError.ReadToEnd();
            run.WaitForExit();
            if (run.ExitCode != 0) throw new InvalidOperationException("server.market: " + errors);
            return output;
        }

        /// <summary>Reads or changes the shared database directly, as the market service does.</summary>
        public long Sql(string statement)
        {
            using Microsoft.Data.Sqlite.SqliteConnection db = Accounts.Open();
            using Microsoft.Data.Sqlite.SqliteCommand run = db.CreateCommand();
            run.CommandText = statement;
            object answer = run.ExecuteScalar();
            return answer is long number ? number : 0;
        }

        public void Dispose()
        {
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            foreach (string file in new[] { Database, Database + "-wal", Database + "-shm" }) File.Delete(file);
        }

        private static string FindRepo()
        {
            var folder = new DirectoryInfo(AppContext.BaseDirectory);
            while (folder != null && !Directory.Exists(Path.Combine(folder.FullName, "unity", "Assets"))) folder = folder.Parent;
            return folder?.FullName ?? throw new DirectoryNotFoundException("Repo root not found");
        }
    }
}
