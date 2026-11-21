using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

namespace Steel.Services
{
    public sealed class CareerFiles
    {
        [Serializable] private sealed class Slot
        {
            public long sequence, utc;
            public string json, hash;
        }
        private readonly string[] paths;
        private int active = -1;
        private long sequence;
        public bool ReadOnly { get; private set; }
        public bool HasCareer => active >= 0;
        public long LastUtc { get; private set; }
        public string Notice { get; private set; }
        public CareerFiles(string folder)
        {
            paths = new[] { Path.Combine(folder, "sukabaja-a.json"), Path.Combine(folder, "sukabaja-b.json") };
        }
        private static string Digest(string text)
        {
            using (var hash = SHA256.Create())
                return Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(text)));
        }
        private Slot Read(int index)
        {
            try
            {
                if (!File.Exists(paths[index])) return null;
                var s = JsonUtility.FromJson<Slot>(File.ReadAllText(paths[index]));
                return s?.json != null && s.hash == Digest(s.json) ? s : null;
            }
            catch (Exception e) { Debug.LogWarning("Slot karier tidak terbaca: " + e.GetType().Name); return null; }
        }
        public LocalGameService Open(IConfigSource config, long now, out double elapsed)
        {
            foreach (var c in Enumerable.Range(0, 2).Select(i => new { i, slot = Read(i) })
                .Where(c => c.slot != null).OrderByDescending(c => c.slot.sequence))
            {
                try
                {
                    var game = LocalGameService.Resume(config, c.slot.json, now, out elapsed);
                    active = c.i; sequence = c.slot.sequence; LastUtc = c.slot.utc;
                    return game;
                }
                catch (Exception e) { Debug.LogWarning("Karier tidak dipulihkan: " + e.GetType().Name); }
            }
            ReadOnly = paths.Any(File.Exists);
            if (ReadOnly) Notice = "Simpanan tidak terbaca. Sesi tamu tidak akan menimpanya.";
            elapsed = 0; return new LocalGameService(config);
        }
        public void Write(LocalGameService game, long now)
        {
            if (ReadOnly) return;
            try
            {
                int next = active == 0 ? 1 : 0;
                long utc = Math.Max(now, LastUtc);
                var json = game.Save(utc);
                var s = new Slot { sequence = sequence + 1, utc = utc, json = json, hash = Digest(json) };
                Directory.CreateDirectory(Path.GetDirectoryName(paths[next]));
                File.WriteAllText(paths[next], JsonUtility.ToJson(s));
                active = next; sequence = s.sequence; LastUtc = utc; Notice = null;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            { Notice = "Gagal menyimpan progres. Periksa ruang penyimpanan."; Debug.LogWarning(Notice); }
        }
    }
}
