using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace Steel.Career.Accounts
{
    /// <summary>The accounts of the market service (server/market): one token opens the market and the career alike.</summary>
    public sealed class AccountBook
    {
        private readonly string source;

        public AccountBook(string databasePath)
        {
            source = new SqliteConnectionStringBuilder { DataSource = databasePath, Mode = SqliteOpenMode.ReadWrite }.ToString();
            using SqliteConnection db = Open();
            Run(db, "CREATE TABLE IF NOT EXISTS careers (trader INTEGER PRIMARY KEY REFERENCES traders, save TEXT NOT NULL, " +
                    "seen_ms INTEGER NOT NULL, seq INTEGER NOT NULL, reply TEXT NOT NULL)");
        }

        public SqliteConnection Open()
        {
            var db = new SqliteConnection(source);
            db.Open();
            Run(db, "PRAGMA busy_timeout=5000");
            return db;
        }

        /// <summary>The account a token belongs to, or 0. Suspended accounts answer -1.</summary>
        public long Find(string token)
        {
            if (string.IsNullOrEmpty(token) || token.Length > 128) return 0;
            using SqliteConnection db = Open();
            using SqliteCommand ask = db.CreateCommand();
            ask.CommandText = "SELECT id, EXISTS(SELECT 1 FROM suspensions WHERE trader = id) FROM traders WHERE token_hash = $hash";
            ask.Parameters.AddWithValue("$hash", SHA256.HashData(Encoding.ASCII.GetBytes(token)));
            using SqliteDataReader row = ask.ExecuteReader();
            return !row.Read() ? 0 : row.GetInt64(1) != 0 ? -1 : row.GetInt64(0);
        }

        private static void Run(SqliteConnection db, string sql)
        {
            using SqliteCommand command = db.CreateCommand();
            command.CommandText = sql; command.ExecuteNonQuery();
        }
    }
}
