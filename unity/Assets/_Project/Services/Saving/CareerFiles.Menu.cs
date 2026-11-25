using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Steel.Services
{
    public sealed partial class CareerFiles
    {
        /// <summary>Suffix of a career set aside by "Karier baru"; kept as a one-generation backup.</summary>
        public const string Retired = ".lama";

        /// <summary>What the start menu shows. Reads the slot Open would resume and changes nothing on disk.</summary>
        public CareerSummary Peek(IConfigSource config)
        {
            foreach (var c in Ranked())
            {
                try
                {
                    var view = LocalGameService.Resume(config, c.slot.json, 0, out _).View;
                    return CareerSummary.Saved(view.Cash, 1 + (int)Math.Floor(view.GameHours / 24), view.Caste, c.slot.utc);
                }
                catch (Exception e) { Debug.LogWarning("Karier tidak terbaca untuk menu: " + e.GetType().Name); }
            }
            return paths.Any(File.Exists) ? CareerSummary.Unreadable : CareerSummary.Empty;
        }

        /// <summary>
        /// Moves both slots aside so the next Open starts a new career. The old career stays on the phone as
        /// one backup generation. False puts back what was moved and leaves the old career in place.
        /// </summary>
        public bool Retire()
        {
            var moved = new List<string>();
            try
            {
                foreach (var path in paths.Where(File.Exists))
                {
                    File.Delete(path + Retired);
                    File.Move(path, path + Retired);
                    moved.Add(path);
                }
                return true;
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Debug.LogWarning("Karier lama tidak bisa dipindahkan: " + e.GetType().Name);
                foreach (var path in moved) Restore(path);
                return false;
            }
        }

        private static void Restore(string path)
        {
            try { File.Move(path + Retired, path); }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            { Debug.LogWarning("Slot karier tidak kembali: " + e.GetType().Name); }
        }
    }
}
