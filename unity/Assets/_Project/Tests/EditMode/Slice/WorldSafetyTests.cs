using System;
using NUnit.Framework;
using Steel.Core.State.Slice;
using Steel.Services;

namespace Steel.Tests.Slice
{
    public sealed class WorldSafetyTests
    {
        [TestCase(-15.01, true)] [TestCase(-15, false)] [TestCase(.3, false)]
        [TestCase(double.NaN, true)] [TestCase(double.NegativeInfinity, true)]
        public void ConfiguredFallRule(double y, bool expected) =>
            Assert.AreEqual(expected, new WorldSafety(new ResourceConfigSource().Read("City")).NeedsRespawn(y));
        [Test] public void InvalidSafetyBoundsAreRejected() =>
            Assert.Throws<FormatException>(() => new WorldSafety(
                "{\"spawnY\":0,\"respawnBelowY\":1,\"boundaryHalfWidth\":54,\"boundaryMinZ\":-54,\"boundaryMaxZ\":64}"));
    }
}
