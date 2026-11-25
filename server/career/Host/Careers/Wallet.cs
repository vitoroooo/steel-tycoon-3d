using System;
using Microsoft.Data.Sqlite;

namespace Steel.Career.Careers
{
    /// <summary>The single purse: the market's cash and HMS 1 for an account (traders) are the career's cash and yard
    /// HMS 1. The market moves them between players; the career moves them everywhere else. Both write the same row,
    /// inside one database transaction each, and "wallets" remembers what the career last wrote so that whatever the
    /// market did since then can be carried into the career.</summary>
    public static class Wallet
    {
        public const string Table = "CREATE TABLE IF NOT EXISTS wallets (trader INTEGER PRIMARY KEY REFERENCES traders, " +
                                    "cash INTEGER NOT NULL, stock INTEGER NOT NULL)";

        /// <summary>Carries the market's lots, contracts and fees since the last look into the career.</summary>
        public static void Bring(SqliteConnection db, long account, Career career)
        {
            using SqliteCommand ask = db.CreateCommand();
            ask.CommandText = "SELECT t.cash - w.cash, t.stock - w.stock FROM traders t JOIN wallets w ON w.trader = t.id WHERE t.id = $id";
            ask.Parameters.AddWithValue("$id", account);
            using SqliteDataReader row = ask.ExecuteReader();
            // a career the market has not met yet brings its own purse and replaces the market's starting one
            if (row.Read() && (row.GetInt64(0) != 0 || row.GetInt64(1) != 0)) career.Settle(row.GetInt64(0), row.GetInt64(1));
        }

        /// <summary>Writes the career's purse and caste where the market reads them.</summary>
        public static void Leave(SqliteConnection db, long account, Career career, long marketHour)
        {
            long cash = (long)Math.Floor(career.Cash), stock = (long)Math.Floor(career.StockKg);
            Run(db, "UPDATE traders SET cash = $cash, stock = $stock WHERE id = $id", account, cash, stock);
            Run(db, "INSERT INTO wallets(trader, cash, stock) VALUES ($id, $cash, $stock) " +
                    "ON CONFLICT(trader) DO UPDATE SET cash = $cash, stock = $stock", account, cash, stock);
            // "since" is when this caste was reached; the market protects a new Kasta 3 from blockades by it
            Run(db, "INSERT INTO standing(trader, caste, since) VALUES ($id, $cash, $stock) " +
                    "ON CONFLICT(trader) DO UPDATE SET caste = $cash, since = $stock WHERE caste != $cash",
                account, career.Caste, marketHour);
        }

        private static void Run(SqliteConnection db, string sql, long account, long first, long second)
        {
            using SqliteCommand put = db.CreateCommand();
            put.CommandText = sql;
            put.Parameters.AddWithValue("$id", account);
            put.Parameters.AddWithValue("$cash", first);
            put.Parameters.AddWithValue("$stock", second);
            put.ExecuteNonQuery();
        }
    }
}
