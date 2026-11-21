using Steel.Core.Scrap;

namespace Steel.Core.State.Slice
{
    public sealed class WorldPile
    {
        public string Id { get; }
        public WorldPoint Position { get; }
        public ScrapPile Scrap { get; private set; }
        public bool Collected { get; internal set; }
        public WorldPile(string id, WorldPoint position, ScrapPile scrap)
        { Id = id; Position = position; Scrap = scrap; }

        internal void Restock()
        {
            if (!Collected) return;
            Scrap = Scrap.Fresh();
            Collected = false;
        }
    }
}
