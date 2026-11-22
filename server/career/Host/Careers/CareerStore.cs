using Microsoft.Data.Sqlite;
using Steel.Career.Accounts;
using Steel.Services.Remote;
using UnityEngine;

namespace Steel.Career.Careers
{
    /// <summary>Careers in the database, one row per account.</summary>
    public sealed class CareerStore
    {
        private readonly AccountBook accounts;
        public CareerStore(AccountBook accounts) { this.accounts = accounts; }

        public CareerRecord Read(long account)
        {
            using SqliteConnection db = accounts.Open();
            using SqliteCommand ask = db.CreateCommand();
            ask.CommandText = "SELECT save, seen_ms, seq, reply FROM careers WHERE trader = $id";
            ask.Parameters.AddWithValue("$id", account);
            using SqliteDataReader row = ask.ExecuteReader();
            if (!row.Read()) return null;
            string reply = row.GetString(3);
            return new CareerRecord
            {
                Save = row.GetString(0), SeenMs = row.GetInt64(1), Seq = row.GetInt64(2),
                Reply = reply.Length == 0 ? null : JsonUtility.FromJson<RemoteReply>(reply)
            };
        }

        public void Write(long account, CareerRecord record)
        {
            using SqliteConnection db = accounts.Open();
            using SqliteCommand put = db.CreateCommand();
            put.CommandText = "INSERT INTO careers(trader, save, seen_ms, seq, reply) VALUES ($id, $save, $seen, $seq, $reply) " +
                              "ON CONFLICT(trader) DO UPDATE SET save = $save, seen_ms = $seen, seq = $seq, reply = $reply";
            put.Parameters.AddWithValue("$id", account);
            put.Parameters.AddWithValue("$save", record.Save);
            put.Parameters.AddWithValue("$seen", record.SeenMs);
            put.Parameters.AddWithValue("$seq", record.Seq);
            put.Parameters.AddWithValue("$reply", record.Reply == null ? "" : JsonUtility.ToJson(record.Reply));
            put.ExecuteNonQuery();
        }
    }
}
