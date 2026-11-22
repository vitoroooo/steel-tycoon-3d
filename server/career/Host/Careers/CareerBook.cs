using System;
using System.Collections.Concurrent;
using Steel.Services;
using Steel.Services.Remote;

namespace Steel.Career.Careers
{
    /// <summary>Every career the server is holding. One batch per account at a time; every answer is on disk before it is sent.</summary>
    public sealed class CareerBook
    {
        private readonly IConfigSource rules;
        private readonly CareerStore store;
        private readonly Func<long> nowMs;
        private readonly MarketClock market;
        private readonly ConcurrentDictionary<long, object> locks = new ConcurrentDictionary<long, object>();
        private readonly ConcurrentDictionary<long, Career> open = new ConcurrentDictionary<long, Career>();

        public CareerBook(IConfigSource rules, CareerStore store, Func<long> nowMs)
        {
            this.rules = rules; this.store = store; this.nowMs = nowMs; market = new MarketClock(rules);
        }

        public RemoteReply Open(long account) => With(account, (career, now) => career.Open(now));
        public RemoteReply Act(long account, RemoteBatch batch) => With(account, (career, now) => career.Act(batch, now));

        /// <summary>Forgets the careers held in memory, as a restart of the server does.</summary>
        public void Close() => open.Clear();

        private RemoteReply With(long account, Func<Career, long, RemoteReply> work)
        {
            lock (locks.GetOrAdd(account, _ => new object()))
            {
                long now = nowMs();
                // one transaction: the market cannot move this account's purse between the look and the write
                using Microsoft.Data.Sqlite.SqliteConnection db = store.Open();
                Accounts.AccountBook.Run(db, "BEGIN IMMEDIATE");
                try
                {
                    if (!store.Has(db, account)) open.TryRemove(account, out _);
                    Career career = open.GetOrAdd(account, _ =>
                    {
                        CareerRecord saved = store.Read(db, account);
                        return saved == null ? new Career(rules, now) : new Career(rules, saved);
                    });
                    Wallet.Bring(db, account, career);
                    RemoteReply reply = work(career, now);
                    Wallet.Leave(db, account, career, market.Hour(db, now));
                    store.Write(db, account, career.Record());
                    Accounts.AccountBook.Run(db, "COMMIT");
                    return reply;
                }
                catch
                {
                    // whatever went wrong, the next call starts again from what is on disk
                    open.TryRemove(account, out _);
                    Accounts.AccountBook.Run(db, "ROLLBACK");
                    throw;
                }
            }
        }
    }
}
