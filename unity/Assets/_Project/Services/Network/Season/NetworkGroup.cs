using System;

namespace Steel.Services
{
    /// <summary>Grup Usaha (PRD 11.3): what the trader keeps across seasons.</summary>
    [Serializable]
    public sealed class NetworkGroup
    {
        public const int SeasonDays = 90, Picks = 3;
        public int points, level = 1;
        /// <summary>True when a veteran has not yet chosen his legacies for this season.</summary>
        public bool choose;
        public string[] picks = new string[0];
        public NetworkSeasonPast[] history = new NetworkSeasonPast[0];
        public NetworkLegacy[] legacies = new NetworkLegacy[0];

        /// <summary>A market server older than seasons leaves these out of the board.</summary>
        internal static void Fill(NetworkBoard board)
        {
            board.group = board.group ?? new NetworkGroup();
            board.group.picks = board.group.picks ?? new string[0];
            board.group.history = board.group.history ?? new NetworkSeasonPast[0];
            board.group.legacies = board.group.legacies ?? new NetworkLegacy[0];
            board.city = board.city ?? "";
        }
    }

    /// <summary>A closed season: the city, what the business was worth there, and its final rank.</summary>
    [Serializable]
    public sealed class NetworkSeasonPast
    {
        public int season, rank, points;
        public long value;
        public string city;
        public string Title => rank == 1 ? "Raja Besi Kota " + city + " Season " + season : "Peringkat " + rank + " di " + city;
    }

    /// <summary>One legacy a veteran may start a new city with, and the group level that opens it.</summary>
    [Serializable]
    public sealed class NetworkLegacy
    {
        public string id, name;
        public int level;
    }

    [Serializable]
    internal sealed class LegacyCommand
    {
        public string key = Guid.NewGuid().ToString("N");
        public string action = "bekal", picks;
    }
}
