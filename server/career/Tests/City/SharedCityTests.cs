using System.Linq;
using Steel.Career.Tests.Support;
using Steel.Career.City;
using Xunit;

namespace Steel.Career.Tests.City
{
    /// <summary>One city for everyone (PRD 1): a phone is told who else is in it and where.</summary>
    public sealed class SharedCityTests
    {
        [Fact] public void TwoPlayersSeeEachOtherWhereTheServerSawThemLast()
        {
            using var town = new Town();
            var budi = new Phone(town, "Budi");
            Assert.Empty(budi.Last.others);
            var sari = new Phone(town, "Sari");
            Assert.Equal(new[] { "Budi" }, sari.Last.others.Select(other => other.name));

            sari.Go(sari.Map.Factory); sari.Play(1); sari.Send();
            budi.Play(1);
            var seen = budi.Send().others.Single();
            Assert.Equal("Sari", seen.name);
            Assert.Equal(sari.Map.Factory.X, seen.x, 3);
            Assert.Equal(sari.Map.Factory.Z, seen.z, 3);
            Assert.Equal("", seen.vehicle);
            Assert.Equal(1, seen.caste);
            Assert.Equal(0, budi.Corrections + sari.Corrections);
        }

        [Fact] public void APlayerWhoStopsReportingLeavesTheCity()
        {
            using var town = new Town();
            var budi = new Phone(town, "Budi");
            var sari = new Phone(town, "Sari");
            for (int beat = 0; beat < 6; beat++) { budi.Play(3); budi.Send(); }
            Assert.True(18000 > TownSquare.GoneMs);
            Assert.Empty(budi.Last.others);
            sari.Play(1); sari.Send();
            budi.Play(1);
            Assert.Single(budi.Send().others);
        }

        [Fact] public void OnlyTheNearestPlayersAreSent()
        {
            using var town = new Town();
            var first = new Phone(town, "Pemain 0");
            for (int i = 1; i <= TownSquare.Shown + 3; i++) new Phone(town, "Pemain " + i);
            first.Play(1);
            Assert.Equal(TownSquare.Shown, first.Send().others.Length);
        }
    }
}
