using System;
using System.IO;
using NUnit.Framework;
using Steel.Services;

namespace Steel.Tests.Launch
{
    public sealed class CareerMenuTests
    {
        private readonly ResourceConfigSource source = new ResourceConfigSource();
        private string folder;
        private string A => Path.Combine(folder, "sukabaja-a.json");
        private string B => Path.Combine(folder, "sukabaja-b.json");
        [SetUp] public void Start() => folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        [TearDown] public void Clean() { if (Directory.Exists(folder)) Directory.Delete(folder, true); }

        /// <summary>A career that walked off the spawn point, saved at the given second.</summary>
        private LocalGameService Played(long utc, int saves = 1)
        {
            var disk = new CareerFiles(folder); var game = disk.Open(source, utc, out _);
            game.Advance(3 * 3600);
            for (int i = 0; i < saves; i++) disk.Write(game, utc + i);
            return game;
        }

        [Test] public void NoFilesIsAFirstCareer() =>
            Assert.AreEqual(CareerState.Empty, new CareerFiles(folder).Peek(source).State);

        [Test] public void SummaryMatchesTheSavedCareerAndReadingItChangesNothing()
        {
            var game = Played(1000);
            string before = File.ReadAllText(A);
            var summary = new CareerFiles(folder).Peek(source);
            Assert.AreEqual(CareerState.Saved, summary.State);
            Assert.AreEqual(game.View.Cash, summary.Cash); Assert.AreEqual(game.View.Caste, summary.Caste);
            Assert.AreEqual(1 + (int)Math.Floor(game.View.GameHours / 24), summary.Day);
            Assert.AreEqual(1000, summary.SavedUtc);
            Assert.AreEqual(before, File.ReadAllText(A)); Assert.IsFalse(File.Exists(B));
            Assert.AreEqual(1, Directory.GetFiles(folder).Length);
        }

        [Test] public void SummaryFallsBackToTheOlderSlotLikeResumeDoes()
        {
            Played(1000, 2); File.WriteAllText(B, "{}");
            var summary = new CareerFiles(folder).Peek(source);
            Assert.AreEqual(CareerState.Saved, summary.State); Assert.AreEqual(1000, summary.SavedUtc);
        }

        [Test] public void DamagedSlotsAreReportedNotHidden()
        {
            Played(1000, 2); File.WriteAllText(A, "{}"); File.WriteAllText(B, "{}");
            Assert.AreEqual(CareerState.Unreadable, new CareerFiles(folder).Peek(source).State);
        }
    }
}
