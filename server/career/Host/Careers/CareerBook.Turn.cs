using System;
using Microsoft.Data.Sqlite;
using Steel.Core.Trade.Town;
using Steel.Services.Remote;

namespace Steel.Career.Careers
{
    public sealed partial class CareerBook
    {
        private string Name(SqliteConnection db, long account)
        {
            if (!names.TryGetValue(account, out string name)) names[account] = name = store.Name(db, account);
            return name;
        }

        private Career Held(SqliteConnection db, long account, long now)
        {
            if (open.TryGetValue(account, out Career held)) return held;
            CareerRecord saved = store.Read(db, account);
            return open[account] = saved == null ? new Career(rules, now) : new Career(rules, saved);
        }

        /// <summary>One career's turn: the market's changes come in, the work runs, what it sold to other players is
        /// booked on their careers, and the result goes back to the market's tables and to disk.</summary>
        private RemoteReply Turn(SqliteConnection db, long account, long now, Func<Career, long, RemoteReply> work)
        {
            if (!store.Has(db, account))
            {
                // the season closed: the career starts again, and its yard is gone
                open.Remove(account); Yards.Drop(account);
                if (work == null) return null;
            }
            Career career = Held(db, account, now);
            Wallet.Bring(db, account, career);
            long[] buyers = Yards.Neighbours(account);
            career.Post(Yards.For(account, out double[] canPay), canPay);
            RemoteReply reply = work?.Invoke(career, now);
            foreach (Delivery note in career.Deliveries())
                Turn(db, buyers[note.plot - City.TownYards.FirstPlot], now, (buyer, _) => { buyer.Take(note); return null; });
            Wallet.Leave(db, account, career, market.Hour(db, now));
            store.Write(db, account, career.Record());
            Yards.Post(account, Name(db, account), career);
            return reply;
        }
    }
}
