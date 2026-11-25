using System;

namespace Steel.Services
{
    /// <summary>The trader's own association (PRD 8.1): its members and the latest chat lines. No name: he has none.</summary>
    [Serializable]
    public sealed class NetworkGuild
    {
        public const int MaxMembers = 10, ChatLength = 120;
        public const long BlockadeCost = 500000000, EscortCost = 50000000;
        /// <summary>The caste packages of PRD A.4: to Kasta 2, then to Kasta 3.</summary>
        public static readonly long[] PromoteCost = {250000000, 2950000000};
        public string name = "";
        public string[] members = new string[0];
        public NetworkChatLine[] messages = new NetworkChatLine[0];
        public bool Joined => !string.IsNullOrEmpty(name);

        /// <summary>A market server older than associations leaves these out of the board.</summary>
        internal static void Fill(NetworkBoard board)
        {
            board.association = board.association ?? new NetworkGuild();
            board.association.members = board.association.members ?? new string[0];
            board.association.messages = board.association.messages ?? new NetworkChatLine[0];
            board.associations = board.associations ?? new NetworkGuildEntry[0];
            board.rivals = board.rivals ?? new string[0];
            board.blockedBy = board.blockedBy ?? "";
            if (board.caste < 1) board.caste = 1;
        }
    }

    [Serializable]
    public sealed class NetworkChatLine
    {
        public long id, hour;
        public string name, text;
    }

    /// <summary>One association of the city, as listed for a trader looking for one to join.</summary>
    [Serializable]
    public sealed class NetworkGuildEntry
    {
        public string name;
        public int members;
    }
}
