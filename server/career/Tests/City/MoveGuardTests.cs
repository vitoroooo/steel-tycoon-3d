using System.Linq;
using Steel.Career.City;
using Steel.Career.Tests.Support;
using Steel.Services;
using Xunit;

namespace Steel.Career.Tests.City
{
    /// <summary>The server does not believe a position a body could not have reached (PRD 12).</summary>
    public sealed class MoveGuardTests
    {
        private static double Apart(Phone phone, SliceLocation place)
        {
            GameView me = phone.Game.View;
            return System.Math.Sqrt(System.Math.Pow(place.X - me.PlayerX, 2) + System.Math.Pow(place.Z - me.PlayerZ, 2));
        }

        private static int Farthest(Phone phone) => Enumerable.Range(0, phone.Map.Piles.Length)
            .OrderByDescending(i => Apart(phone, phone.Map.Piles[i].Position)).First();

        [Fact] public void APlayerWhoWalksIsBelievedAllTheWay()
        {
            using var town = new Town(guarded: true);
            var phone = new Phone(town, "Sari");
            int far = Farthest(phone);
            SliceLocation pile = phone.Map.Piles[far].Position;
            Assert.True(Apart(phone, pile) > MoveGuard.Slack * 2, "The pile is a real walk away");
            phone.Walk(pile);
            phone.Do("Inspect", "p" + far); phone.Do("PickUp", "p" + far);
            Assert.All(phone.Send().results, result => Assert.True(result.ok, result.error));
            Assert.True(phone.Game.View.CarriedKg > 0);
            Assert.Equal(0, phone.Corrections);
        }

        [Fact] public void AJumpIsRefusedAndWhatItWasForDoesNotHappen()
        {
            using var town = new Town(guarded: true);
            var phone = new Phone(town, "Sari");
            phone.Play(2);
            int far = Farthest(phone);
            phone.Go(phone.Map.Piles[far].Position); phone.Do("Inspect", "p" + far); phone.Do("PickUp", "p" + far);
            Assert.True(phone.Game.View.CarriedKg > 0, "The phone's own copy believes itself");
            var reply = phone.Send();
            Assert.Equal(MoveGuard.Refusal, reply.results[0].error);
            Assert.All(reply.results, result => Assert.False(result.ok));
            Assert.Equal(1, phone.Corrections);
            Assert.Equal(0, phone.Game.View.CarriedKg);
            Assert.True(Apart(phone, phone.Map.Spawn) < 0.01, "The server kept the player where he was");
        }

        [Fact] public void StandingStillBuysWalkingDistanceAndNoMore()
        {
            using var town = new Town(guarded: true);
            var phone = new Phone(town, "Sari");
            SliceLocation start = phone.Map.Spawn;
            double seconds = 60, walked = phone.Map.MoveSpeed * seconds;
            for (int i = 0; i < 20; i++) { phone.Play(seconds / 20); phone.Send(); }
            phone.Do("SetPosition", start.X + (float)(walked * 2), start.Z);
            Assert.False(phone.Send().results.Single().ok);
            phone.Do("SetPosition", start.X + (float)walked, start.Z);
            Assert.True(phone.Send().results.Single().ok);
        }

        [Fact] public void AParkedVehicleStaysWhereItStands()
        {
            using var town = new Town(guarded: true);
            var phone = new Phone(town, "Sari");
            VehicleView truck = phone.Game.VehicleOf(FleetCatalog.Engkel);
            phone.Do("ReportVehicle", FleetCatalog.Engkel, truck.X + 30, truck.Z, 0f, (float?)90f);
            Assert.True(phone.Send().results.Single().ok, "The scene places a vehicle once");
            phone.Play(600);
            phone.Do("ReportVehicle", FleetCatalog.Engkel, truck.X + 31, truck.Z, 0f, (float?)90f);
            Assert.True(phone.Send().results.Single().ok, "Settling on its wheels is not a move");
            phone.Do("ReportVehicle", FleetCatalog.Engkel, truck.X + 300, truck.Z, 0f, (float?)90f);
            Assert.Equal(MoveGuard.Refusal, phone.Send().results.Single().error);
            phone.Do("ReportVehicle", FleetCatalog.Engkel, truck.X, truck.Z, 0f, (float?)90f);
            Assert.True(phone.Send().results.Single().ok, "A vehicle that fell off the map is put back at the dealer");
        }
    }
}
