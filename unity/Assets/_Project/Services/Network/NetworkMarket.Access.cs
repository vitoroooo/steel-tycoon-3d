using System;

namespace Steel.Services
{
    /// <summary>Where a market is and whose account it is.</summary>
    public sealed partial class NetworkMarket
    {
        /// <summary>True when this is the account the online career is played on: its cash is the career's cash.</summary>
        public bool Shared { get; }

        public NetworkMarket(string origin) { endpoint = MarketEndpoint.ForHost(origin); }

        /// <summary>The market of an account already signed in to the career server beside it.</summary>
        internal NetworkMarket(Uri endpoint, string token) { this.endpoint = endpoint; this.token = token; Shared = true; }

        /// <summary>A market on this machine, for testing.</summary>
        public NetworkMarket(int port = 8765)
        {
            if (port < 1 || port > 65535) throw new ArgumentOutOfRangeException(nameof(port));
            endpoint = new UriBuilder("http", "127.0.0.1", port, "v1/").Uri;
        }
    }
}
