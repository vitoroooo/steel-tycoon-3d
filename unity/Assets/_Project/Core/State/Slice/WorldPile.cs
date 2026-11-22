using Steel.Core.Scrap;

namespace Steel.Core.State.Slice
{
    public sealed class WorldPile
    {
        public string Id { get; }
        public WorldPoint Position { get; }
        public ScrapPile Scrap { get; private set; }
        public bool Collected { get; private set; }
        /// <summary>Queue number handed out when the site was emptied; the lowest refills first.</summary>
        public long EmptiedOrder { get; private set; }
        public WorldPile(string id, WorldPoint position, ScrapPile scrap)
        { Id = id; Position = position; Scrap = scrap; }

        internal void Empty(long order) { Collected = true; EmptiedOrder = order; }

        internal void Restock()
        {
            if (!Collected) return;
            Scrap = Scrap.Fresh();
            Collected = false; EmptiedOrder = 0;
        }
    }
}
