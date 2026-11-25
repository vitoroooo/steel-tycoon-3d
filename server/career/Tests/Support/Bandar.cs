using Steel.Career.Careers;
using Steel.Core.State.Saving;
using UnityEngine;

namespace Steel.Career.Tests.Support
{
    /// <summary>Makes a saved career a trading yard without playing the weeks that takes, then restarts the server.</summary>
    public static class Bandar
    {
        public static void Open(Town town, string name, double cash, params double[] bids)
        {
            long account = town.Sql("SELECT id FROM traders WHERE name = '" + name + "'");
            var store = new CareerStore(town.Accounts);
            using (Microsoft.Data.Sqlite.SqliteConnection db = town.Accounts.Open())
            {
                CareerRecord record = store.Read(db, account);
                CareerSave save = JsonUtility.FromJson<CareerSave>(record.Save);
                save.cash = cash;
                save.yard = new YardSave { reputation = 40, bandar = true, applied = true, leased = true, leaseUntil = 1e6, bids = bids };
                record.Save = JsonUtility.ToJson(save);
                store.Write(db, account, record);
            }
            town.Careers.Close();
        }
    }
}
