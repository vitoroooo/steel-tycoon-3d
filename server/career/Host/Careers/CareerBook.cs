using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Steel.Core.Trade.Town;
using Steel.Services;
using Steel.Services.Remote;

namespace Steel.Career.Careers
{
    /// <summary>Every career the server is holding. One batch at a time, because a sale between players changes two
    /// careers at once; every answer is on disk before it is sent.</summary>
    public sealed partial class CareerBook
    {
        private const string Sell = "SellToPlayerYard";
        private readonly IConfigSource rules;
        private readonly CareerStore store;
        private readonly Func<long> nowMs;
        private readonly MarketClock market;
        private readonly object gate = new object();
        private readonly Dictionary<long, Career> open = new Dictionary<long, Career>();
        private readonly Dictionary<long, string> names = new Dictionary<long, string>();
        private bool listed;
        /// <summary>The shared city: every answer also tells a phone who else is in it and which yards are buying.</summary>
        public City.TownSquare Square { get; } = new City.TownSquare();
        public City.TownYards Yards { get; } = new City.TownYards();
        /// <summary>True believes every position a phone reports. Only for tests that put a player somewhere at once.</summary>
        public bool Roaming { get; set; }

        public CareerBook(IConfigSource rules, CareerStore store, Func<long> nowMs)
        {
            this.rules = rules; this.store = store; this.nowMs = nowMs; market = new MarketClock(rules);
        }

        public RemoteReply Open(long account) => With(account, false, (career, now) => career.Open(now));
        public RemoteReply Act(long account, RemoteBatch batch) =>
            With(account, batch.calls?.Any(call => call?.name == Sell) ?? false, (career, now) => career.Act(batch, now));

        /// <summary>Forgets the careers held in memory, as a restart of the server does.</summary>
        public void Close() { lock (gate) Forget(); }
        private void Forget() { open.Clear(); Yards.Clear(); listed = false; }

        private RemoteReply With(long account, bool selling, Func<Career, long, RemoteReply> work)
        {
            lock (gate)
            {
                long now = nowMs();
                // one transaction: the market cannot move an account's purse between the look and the write
                using SqliteConnection db = store.Open();
                Accounts.AccountBook.Run(db, "BEGIN IMMEDIATE");
                try
                {
                    if (!listed) { foreach (long id in store.All(db)) Yards.Post(id, Name(db, id), Held(db, id, now)); listed = true; }
                    // before a sale, the buyers' purses are read again: the market may have moved them
                    if (selling) foreach (long buyer in Yards.Neighbours(account)) Turn(db, buyer, now, null);
                    RemoteReply reply = Turn(db, account, now, work);
                    Accounts.AccountBook.Run(db, "COMMIT");
                    Square.Show(account, Name(db, account), open[account], now);
                    return reply.Among(Square.Others(account, now), Yards.For(account, out _));
                }
                catch
                {
                    // whatever went wrong, the next call starts again from what is on disk
                    Forget();
                    Accounts.AccountBook.Run(db, "ROLLBACK");
                    throw;
                }
            }
        }
    }
}
