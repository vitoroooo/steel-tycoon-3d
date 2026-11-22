using System.Collections.Concurrent;
using System.Linq;
using Steel.Services.Remote;

namespace Steel.Career.City
{
    /// <summary>Who is in the city right now. Every batch a phone sends says where its player is; whoever has been
    /// silent for a while has left. Kept in memory only: after a restart the city fills again within a few seconds.</summary>
    public sealed class TownSquare
    {
        /// <summary>A player is gone this long after his last batch; a phone reports every three seconds.</summary>
        public const long GoneMs = 15000;
        public const int Shown = 12;
        private sealed class Seen { public RemotePlayer Player; public long AtMs; }
        private readonly ConcurrentDictionary<long, Seen> here = new ConcurrentDictionary<long, Seen>();

        public void Show(long account, string name, Careers.Career career, long nowMs) => here[account] = new Seen
        {
            AtMs = nowMs,
            Player = new RemotePlayer { name = name, x = career.X, z = career.Z, vehicle = career.Vehicle, caste = career.Caste }
        };

        /// <summary>The players nearest to this one, himself left out.</summary>
        public RemotePlayer[] Others(long account, long nowMs)
        {
            if (!here.TryGetValue(account, out Seen me)) return new RemotePlayer[0];
            foreach (var old in here.Where(seen => nowMs - seen.Value.AtMs > GoneMs).ToArray()) here.TryRemove(old.Key, out _);
            return here.Where(seen => seen.Key != account).Select(seen => seen.Value.Player)
                .OrderBy(other => (other.x - me.Player.x) * (other.x - me.Player.x) + (other.z - me.Player.z) * (other.z - me.Player.z))
                .Take(Shown).ToArray();
        }
    }
}
