using Steel.Core.State;
using Steel.Core.State.Slice;

namespace Steel.Services
{
    public sealed class CityLayout
    {
        public SliceLocation Home { get; }
        public SliceLocation Factory { get; }
        public SliceLocation Spawn { get; }
        public float MoveSpeed { get; }
        public float LookSensitivity { get; }
        public float CameraDistance { get; }
        public float CameraHeight { get; }
        public PileLocation[] Piles { get; }
        public CityLayout(IConfigSource source)
        {
            var text = source.Read("City"); var v = new ConfigValues(text);
            var rules = new SliceRules(text);
            Home = new SliceLocation(rules.Home); Factory = new SliceLocation(rules.Factory);
            Spawn = new SliceLocation(rules.Spawn);
            MoveSpeed = (float)v.Positive("moveSpeed"); LookSensitivity = (float)v.Positive("lookSensitivity");
            CameraDistance = (float)v.Positive("cameraDistance"); CameraHeight = (float)v.Positive("cameraHeight");
            var world = new SliceWorld(source.Read("Piles"));
            Piles = new PileLocation[world.Piles.Count];
            for (int i = 0; i < Piles.Length; i++)
                Piles[i] = new PileLocation(world.Piles[i].Id, new SliceLocation(world.Piles[i].Position));
        }
    }
    public readonly struct SliceLocation
    {
        public float X { get; }
        public float Z { get; }
        internal SliceLocation(WorldPoint point) { X = (float)point.X; Z = (float)point.Z; }
    }
    public readonly struct PileLocation
    {
        public string Id { get; }
        public SliceLocation Position { get; }
        internal PileLocation(string id, SliceLocation position) { Id = id; Position = position; }
    }
}
