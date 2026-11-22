using System;
using System.IO;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Launch
{
    public sealed class CareerStartTests
    {
        private readonly ResourceConfigSource source = new ResourceConfigSource();
        private string folder;
        private string A => Path.Combine(folder, "sukabaja-a.json");
        private string B => Path.Combine(folder, "sukabaja-b.json");
        [SetUp] public void Start() => folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        [TearDown] public void Clean() { if (Directory.Exists(folder)) Directory.Delete(folder, true); }

        private double PlayAndSave(long utc, double seconds)
        {
            var start = CareerStart.Open(folder, source, utc, false);
            start.Game.Advance(seconds); start.Files.Write(start.Game, utc); start.Files.Write(start.Game, utc + 1);
            return start.Game.View.GameHours;
        }

        [Test] public void ContinueResumesTheSavedCareer()
        {
            double hours = PlayAndSave(1000, 7200);
            var start = CareerStart.Open(folder, source, 1001, false);
            Assert.IsTrue(start.Files.HasCareer); Assert.IsFalse(start.Replaced); Assert.IsNull(start.Notice);
            Assert.AreEqual(hours, start.Game.View.GameHours, 1e-6);
        }

        [Test] public void NewCareerStartsFromZeroAndKeepsTheOldOneAsBackup()
        {
            PlayAndSave(1000, 7200);
            string a = File.ReadAllText(A), b = File.ReadAllText(B);
            var start = CareerStart.Open(folder, source, 2000, true);
            Assert.IsTrue(start.Replaced); Assert.IsFalse(start.Files.HasCareer); Assert.IsFalse(start.Files.ReadOnly);
            Assert.AreEqual(new LocalGameService(source).View.GameHours, start.Game.View.GameHours, 1e-9);
            Assert.AreEqual(a, File.ReadAllText(A + CareerFiles.Retired));
            Assert.AreEqual(b, File.ReadAllText(B + CareerFiles.Retired));
            Assert.IsFalse(File.Exists(A)); Assert.IsFalse(File.Exists(B));
            start.Files.Write(start.Game, 2000);
            var summary = new CareerFiles(folder).Peek(source);
            Assert.AreEqual(CareerState.Saved, summary.State); Assert.AreEqual(2000, summary.SavedUtc);
        }

        [Test] public void NewCareerOverAnUnreadableSaveCanBeSavedAgain()
        {
            Directory.CreateDirectory(folder); File.WriteAllText(A, "{}"); File.WriteAllText(B, "rusak");
            Assert.IsTrue(CareerStart.Open(folder, source, 1000, false).Files.ReadOnly, "Guest session");
            var start = CareerStart.Open(folder, source, 1000, true);
            Assert.IsFalse(start.Files.ReadOnly); start.Files.Write(start.Game, 1000);
            Assert.AreEqual(CareerState.Saved, new CareerFiles(folder).Peek(source).State);
            Assert.AreEqual("rusak", File.ReadAllText(B + CareerFiles.Retired));
        }

        [Test] public void SecondNewCareerKeepsOnlyTheLatestBackup()
        {
            PlayAndSave(1000, 7200); CareerStart.Open(folder, source, 2000, true);
            PlayAndSave(3000, 3600);
            string latest = File.ReadAllText(A);
            Assert.IsTrue(CareerStart.Open(folder, source, 4000, true).Replaced);
            Assert.AreEqual(latest, File.ReadAllText(A + CareerFiles.Retired));
        }

        [Test] public void NewCareerOnAnEmptyPhoneCreatesNothing()
        {
            var start = CareerStart.Open(folder, source, 1000, true);
            Assert.IsTrue(start.Replaced); Assert.IsNull(start.Notice); Assert.IsFalse(Directory.Exists(folder));
        }

        [Test] public void OldCareerStaysWhenItCannotBeMoved()
        {
            PlayAndSave(1000, 7200);
            Directory.CreateDirectory(B + CareerFiles.Retired);
            File.WriteAllText(Path.Combine(B + CareerFiles.Retired, "isi"), "x");
            var start = CareerStart.Open(folder, source, 2000, true);
            Assert.IsFalse(start.Replaced); Assert.AreEqual(CareerStart.KeptNotice, start.Notice);
            Assert.IsTrue(start.Files.HasCareer, "Both slots are back, the career continues");
            Assert.IsTrue(File.Exists(A)); Assert.IsTrue(File.Exists(B));
        }
    }
}
